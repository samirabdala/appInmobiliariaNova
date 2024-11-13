using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using inmobiliaria_AT.Models;

namespace Inmobiliaria_.Net_Core.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InmuebleController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<InmuebleController> _logger;

        public InmuebleController(AppDbContext context, ILogger<InmuebleController> logger)
        {
            _context = context;
            _logger = logger;
        }


        // GET: api/inmueble
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Inmueble>>> GetAll()
        {
            // Obtén el ID del propietario logueado
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int parsedUserId))
            {
                return BadRequest("El ID del propietario no es válido.");
            }

            var inmueble = await _context.Inmueble
                .Where(i => i.IdPropietario == parsedUserId)
                .Include(i => i.Tipo)
                .ToListAsync();


            return Ok(inmueble);
        }

        // GET: api/inmueble/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Inmueble>> GetById(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int parsedUserId))
            {
                return BadRequest("El ID del propietario no es válido.");
            }

            var inmueble = await _context.Inmueble
                .Include(i => i.Tipo)
                .FirstOrDefaultAsync(i => i.Id == id && i.IdPropietario == parsedUserId); // verifica que el inmueble pertenezca al propietario

            if (inmueble == null)
            {
                return NotFound();
            }

            return Ok(inmueble);
        }

        // POST: api/inmueble/nuevo
        [HttpPost("nuevo")]
        public async Task<ActionResult<Inmueble>> Nuevo([FromBody] Inmueble entidad)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int parsedUserId))
            {
                return BadRequest("El ID del propietario no es válido.");
            }
            entidad.IdPropietario = parsedUserId;

            _context.Entry(entidad.Tipo).State = EntityState.Unchanged;

            if (ModelState.IsValid)
            {
                _context.Inmueble.Add(entidad);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = entidad.Id }, entidad);
            }
            return BadRequest(ModelState);
        }


        [HttpPatch("cambiarEstado/{id}")]
        public async Task<IActionResult> CambiarDisponibilidad([FromRoute] int id, [FromForm] bool estado)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int parsedUserId))
            {
                return BadRequest("El ID del propietario no es válido.");
            }

            var inmueble = await _context.Inmueble
                .FirstOrDefaultAsync(i => i.Id == id && i.IdPropietario == parsedUserId);

            if (inmueble == null)
            {
                return NotFound(new { mensaje = "Inmueble no encontrado o no pertenece al usuario." });
            }
           
            inmueble.Estado = estado;
            _context.Inmueble.Update(inmueble);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Estado actualizado exitosamente" });
        }


        [HttpGet("tipos")]
        public async Task<ActionResult<IEnumerable<Tipo>>> GetTipos()
        {
            var tipos = await _context.Tipo.ToListAsync();
            return Ok(tipos);
        }

    }
}
