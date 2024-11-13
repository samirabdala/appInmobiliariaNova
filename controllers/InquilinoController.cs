
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
    public class InquilinoController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<InquilinoController> _logger;

        public InquilinoController(AppDbContext context, ILogger<InquilinoController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/inquilino
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contrato>>> GetAll()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int parsedUserId))
            {
                return BadRequest("El ID del propietario no es válido.");
            }

            var currentDate = DateTime.Now.Date;

            var contratos = await _context.Contrato
                .Where(c => c.Inmu.IdPropietario == parsedUserId &&
                            c.FechaInicio <= currentDate &&
                            c.FechaFin >= currentDate)
                .Include(c => c.Inqui) // Incluye la info del inquilino
                .Include(c => c.Inmu) // Incluye la info del inmueble
                .ToListAsync();

            if (contratos == null || contratos.Count == 0)
            {
                return NotFound("No se encontraron contratos.");
            }
            return Ok(contratos);
        }





        // GET: api/inquilino/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetById(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int parsedUserId))
            {
                return BadRequest("El ID del propietario no es válido.");
            }

            var currentDate = DateTime.Now;

            var inquilino = await _context.Contrato
                .Where(c => c.Prop.Id == parsedUserId &&
                            c.Inqui.Id == id &&
                            c.FechaInicio <= currentDate &&
                            c.FechaFin >= currentDate)
                .Include(c => c.Inqui)
                .Include(c => c.Inmu)
                .Select(c => new
                {
                    Inquilino = c.Inqui,
                    Inmueble = c.Inmu
                })
                .FirstOrDefaultAsync();

            if (inquilino == null)
            {
                return NotFound();
            }

            return Ok(inquilino);
        }

    }
}
