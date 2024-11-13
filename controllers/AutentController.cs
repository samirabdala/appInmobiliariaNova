using inmobiliaria_AT.Models;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Policy;
using System.Text;

namespace inmobiliaria_AT.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IPropietarioService _propietarioService;
        private readonly AppDbContext contexto;

        private readonly IWebHostEnvironment environment;

        public AuthController(IConfiguration configuration, IPropietarioService propietarioService, AppDbContext contexto, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _propietarioService = propietarioService;
            this.contexto = contexto;
            this.environment = environment;
        }

        [HttpPost("login")]

        public IActionResult Login([FromBody] LoginView loginView)
        {
            var propietarioDb = _propietarioService.ValidarPropietario(loginView.Email, loginView.Password);
            if (propietarioDb == null)
            {
                Console.WriteLine("Propietario no encontrado"); ////////////
                return Unauthorized();

            }

            // Generación de claims
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, propietarioDb.Id.ToString()),
                new Claim(ClaimTypes.Name, propietarioDb.Email),
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["TokenAuthentication:Issuer"],
                audience: _configuration["TokenAuthentication:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMonths(1),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["TokenAuthentication:SecretKey"])),
                    SecurityAlgorithms.HmacSha256));

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new
            {
                Token = "Bearer " + tokenString,
                Expiracion = token.ValidTo,
                NombreCompleto = propietarioDb.Nombre + " " + propietarioDb.Apellido,
                Email = propietarioDb.Email
                ,
                Avatar = propietarioDb.Avatar
            });
        }


        // GET api/<controller>/email

        [HttpPost("email")]
        public async Task<IActionResult> GetByEmail([FromForm] string email)
        {
            try
            {
                // Buscar el propietario por email
                var propietarioDb = await contexto.Propietario.FirstOrDefaultAsync(x => x.Email == email);

                if (propietarioDb == null)
                {
                    return NotFound("El propietario no existe.");
                }

                // Dominio sirve para armar el enlace
                var claims = new[]
                {
            new Claim(ClaimTypes.NameIdentifier, propietarioDb.Id.ToString()),
            new Claim(ClaimTypes.Name, propietarioDb.Email)
        };

                // Crear el token JWT para el enlace de reseteo de contraseña
                var token = new JwtSecurityToken(
                    issuer: _configuration["TokenAuthentication:Issuer"],
                    audience: _configuration["TokenAuthentication:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(15),
                    signingCredentials: new SigningCredentials(
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["TokenAuthentication:SecretKey"])),
                        SecurityAlgorithms.HmacSha256)
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                var dominio = environment.IsDevelopment()
                    ? HttpContext.Connection.LocalIpAddress?.MapToIPv4().ToString()
                    : "www.otro.com";

                var resetLink = $"http://{dominio}:5181/api/auth/restablecer_password?token={tokenString}";

                // Enviar el enlace por correo
                var message = new MimeMessage();
                message.To.Add(new MailboxAddress(propietarioDb.Nombre, propietarioDb.Email));
                message.From.Add(new MailboxAddress("NOVA INMOBILIARIA", _configuration["EmailSettings:SMTPUser"]));
                message.Subject = "Restablecimiento de contraseña";
                message.Body = new TextPart("html")
                {
                    Text = @$"<h1>Restablecimiento de Contraseña</h1>
                      <p>Hola {propietarioDb.Nombre},</p>
                      <p>Se ha solicitado restablecer la contraseña de tu cuenta NOVA INMOBILIARIA.</p>
                      <p>Para restablecer tu contraseña, haz clic en el siguiente enlace:</p>
                      <a href='{resetLink}'>Restablecer Contraseña</a>
                      <p><strong>Al hacer clic en el enlace, serás redirigido a tu aplicación para el cambio de contraseña,
                      donde deberas ingresar la contraseña que te enviaremos a continuación.</strong> </p>
                      <p>Si no has solicitado este proceso, puedes ignorar este correo.</p>",
                };

                using (var client = new SmtpClient())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true; // Ignorar validación SSL
                    await client.ConnectAsync(_configuration["EmailSettings:SMTPHost"], int.Parse(_configuration["EmailSettings:SMTPPort"]), MailKit.Security.SecureSocketOptions.StartTls); // Asegúrate de convertir a int
                    await client.AuthenticateAsync(_configuration["EmailSettings:SMTPUser"], _configuration["EmailSettings:SMTPPass"]); // Usa la contraseña de aplicación aquí
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                Console.WriteLine($"Dominio/IP utilizada: {dominio}"); // Log para ver cuál IP se está usando

                return Ok(new { Mensaje = "Enlace de restablecimiento enviado a tu correo.", ResetLink = resetLink });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("restablecer_password")]
        [Authorize]
        public async Task<IActionResult> RestablecerPassword()
        {
            try
            {
                // obtiene el token de la cabecera
                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                var claims = new JwtSecurityTokenHandler().ReadJwtToken(token);
                var emailClaim = claims.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

                var propietarioDb = await contexto.Propietario.FirstOrDefaultAsync(x => x.Email == emailClaim);

                if (propietarioDb == null)
                {
                    return NotFound("El propietario no existe.");
                }

                Random rand = new Random(Environment.TickCount);
                string randomChars = "ABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
                string nuevaClave = new string(Enumerable.Repeat(randomChars, 8)
                    .Select(s => s[rand.Next(s.Length)]).ToArray());

                propietarioDb.Password = HashPassword(nuevaClave);

                // actualiza la base
                await contexto.SaveChangesAsync();

                // envio de correo
                var message = new MimeMessage();
                message.To.Add(new MailboxAddress(propietarioDb.Nombre, propietarioDb.Email));
                message.From.Add(new MailboxAddress("NOVA INMOBILIARIA", _configuration["EmailSettings:SMTPUser"]));
                message.Subject = "Contraseña Restablecida";
                message.Body = new TextPart("html")
                {
                    Text = @$"<h1>Contraseña Restablecida</h1>
                      <p>Hola {propietarioDb.Nombre},</p>
                      <p>Tu nueva contraseña es: <strong>{nuevaClave}</strong></p>
                      <p>Te recomendamos que por tu seguridad, la cambies al acceder a tu cuenta.</p>",
                };

                using (var client = new SmtpClient())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    await client.ConnectAsync(_configuration["EmailSettings:SMTPHost"], int.Parse(_configuration["EmailSettings:SMTPPort"]), MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(_configuration["EmailSettings:SMTPUser"], _configuration["EmailSettings:SMTPPass"]);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                return Ok(new { Mensaje = "Nueva contraseña enviada por correo." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //PATCH api/<controller>/cambiar_password
        [HttpPatch("cambiar_password")]
        [Authorize]
        public async Task<IActionResult> CambiarPassword([FromForm] String password, [FromForm] String passwordActual)
        {
            try
            {
                // Obtener el token de la cabecera
                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                // Decodificar el token para obtener el correo o ID del propietario (se asume que el token contiene esta información)                
                var claims = new JwtSecurityTokenHandler().ReadJwtToken(token);
                var emailClaim = claims.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

                // Buscar al propietario por email
                var propietarioDb = await contexto.Propietario.FirstOrDefaultAsync(x => x.Email == emailClaim);

                if (propietarioDb == null)
                {
                    return NotFound("El propietario no existe.");
                }

                if (!BCrypt.Net.BCrypt.Verify(passwordActual, propietarioDb.Password))
                {
                    return BadRequest("La contraseña actual no coincide.");
                }


                propietarioDb.Password = HashPassword(password);

                await contexto.SaveChangesAsync(); //ACTUALIZA LA BASE DE DATOS
                var message = new MimeMessage();
                message.To.Add(new MailboxAddress(propietarioDb.Nombre, propietarioDb.Email));
                message.From.Add(new MailboxAddress("NOVA INMOBILIARIA", _configuration["EmailSettings:SMTPUser"]));
                message.Subject = "Cambio de contraseña";
                message.Body = new TextPart("html")
                {
                    Text = @$"<h1>Cambio de contraseña</h1>
                      <p>Hola {propietarioDb.Nombre},</p>
                      <p>Has modificado satisfactoriamente la contraseña de tu cuenta de NOVA INMOBILIARIA.</p>
                      <p>Si no has sido tu el que realizó el cambio, te recordamos que solicites un nuevo restablecimiento de contraseña de inmediato.</p>
                      ",
                };

                using (var client = new SmtpClient())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true; // Ignorar validación SSL
                    await client.ConnectAsync(_configuration["EmailSettings:SMTPHost"], int.Parse(_configuration["EmailSettings:SMTPPort"]), MailKit.Security.SecureSocketOptions.StartTls); // Asegúrate de convertir a int
                    await client.AuthenticateAsync(_configuration["EmailSettings:SMTPUser"], _configuration["EmailSettings:SMTPPass"]); // Usa la contraseña de aplicación aquí
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                return Ok(new { Mensaje = "Contraseña cambiada con éxito." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // para hashear la contraseña
        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password); // Sin salt manual
        }


       

    }

}
