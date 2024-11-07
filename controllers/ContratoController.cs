
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
    public class ContratoController : ControllerBase
    {
        private readonly AppDbContext _context; 
        private readonly ILogger<InquilinoController> _logger;


        public ContratoController(AppDbContext context, ILogger<InquilinoController> logger)
        {
            _context = context;
            _logger = logger;
        }


        // GET: api/Contrato
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contrato>>> GetAll()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int parsedUserId))
            {
                return BadRequest("El ID del propietario no es válido.");
            }

            var currentDateTime = DateTime.UtcNow;


            var contratos = await _context.Contrato
                .Where(c => c.PropId == parsedUserId &&
                            c.FechaInicio <= currentDateTime &&
                            c.FechaFin >= currentDateTime)
                .Include(c => c.Inqui) // Incluye la información del inquilino
                .Include(c => c.Inmu) // Incluye la información del inmueble
                .ToListAsync();

            // Agrega registros de depuración
            _logger.LogInformation("Contratos encontrados: {contratosCount}", contratos.Count);

            if (contratos == null || contratos.Count == 0)
            {
                return NotFound("No se encontraron contratos.");
            }

            return Ok(contratos);
        }




        // GET: api/Contrato/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Contrato>> GetById(int id)
        {
            var contrato = await _context.Contrato
                .Include(c => c.Inqui)
                .Include(c => c.Inmu)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contrato == null)
            {
                return NotFound();
            }

            return Ok(contrato);
        }

        // POST: api/Contrato
        [HttpPost]
        public async Task<ActionResult<Contrato>> Create(Contrato contrato)
        {
            if (contrato == null)
            {
                return BadRequest("Contrato no puede ser nulo.");
            }

            _context.Contrato.Add(contrato);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = contrato.Id }, contrato);
        }

        // PUT: api/Contrato/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Contrato contrato)
        {
            if (id != contrato.Id)
            {
                return BadRequest("El ID del contrato no coincide.");
            }

            _context.Entry(contrato).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContratoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

       

        private bool ContratoExists(int id)
        {
            return _context.Contrato.Any(e => e.Id == id);
        }
    }
}
