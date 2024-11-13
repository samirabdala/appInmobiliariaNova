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
        public async Task<IActionResult> GetPropietario()
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


            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int parsedUserId))
            {
                return BadRequest("El ID del propietario no es válido.");
            }

            // Busca el propietario por su email
            var propietarioExistente = await _context.Propietario.FirstOrDefaultAsync(p => p.Id == parsedUserId);
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





    }
}
