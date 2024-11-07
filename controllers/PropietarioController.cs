using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using inmobiliaria_AT.Models;
using inmobiliaria_AT;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Humanizer;

namespace inmobiliaria_AT.Controllers
{

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]

    public class PropietarioController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PropietarioController> _logger;

        public PropietarioController(ILogger<PropietarioController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // GET: api/propietario
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("El ID del propietario no puede ser nulo o vacío.");
            }

            if (!int.TryParse(userId, out int parsedUserId))
            {
                return BadRequest("El ID del propietario debe ser un número válido.");
            }

            var propietario = await _context.Propietario
                .FirstOrDefaultAsync(p => p.Id == parsedUserId);

            if (propietario == null)
            {
                return NotFound();
            }
            propietario.Password = null;
            return Ok(propietario);
        }


        [HttpPut]
        public async Task<IActionResult> Actualizar([FromForm] Propietario propietario)
        {
            // Busca el propietario por su email
            var propietarioExistente = await _context.Propietario.FirstOrDefaultAsync(p => p.Email == propietario.Email);
            if (propietarioExistente == null)
            {
                return NotFound(); 
            }

            propietarioExistente.Nombre = propietario.Nombre;
            propietarioExistente.Apellido = propietario.Apellido;
            propietarioExistente.Telefono = propietario.Telefono;
            propietarioExistente.Documento = propietario.Documento;
            propietarioExistente.Direccion = propietario.Direccion; 
            propietarioExistente.Avatar = propietario.Avatar; 

            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpPatch("avatar")]
        public async Task<IActionResult> Avatar(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No se ha seleccionado ningún archivo.");
            }

            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var propietario = await _context.Propietario.SingleOrDefaultAsync(x => x.Email == email);

            if (propietario == null)
            {
                return NotFound("Propietario no encontrado.");
            }

            // creo un directorio para almacenar las imágenes, si no existe
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Genero un nombre unico para el archivo
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream); // Guarda la imagen en el sistema de archivos
                }
            }
            catch (Exception e)
            {
                return BadRequest("Error al guardar la imagen: " + e.Message);
            }

            // Establece la URL del avatar
            var imageUrl = $"/uploads/{fileName}";
            propietario.Avatar = imageUrl; 

            try
            {
                await _context.SaveChangesAsync(); 
            }
            catch (Exception e)
            {
                return BadRequest("Error al actualizar el avatar: " + e.Message);
            }

            return Ok(new { Url = imageUrl }); // retorna la url
        }





    }
}
