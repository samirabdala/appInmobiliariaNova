
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using inmobiliaria_AT.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace inmobiliaria_AT.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]

    public class PagoController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PagoController> _logger;


        public PagoController(ILogger<PagoController> logger, AppDbContext context)
        {

            _logger = logger;
            _context = context;
        }

        [HttpGet("{idContrato}")]
        public async Task<IActionResult> GetPagosPorContrato(int idContrato)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int parsedUserId))
                {
                    return BadRequest(new { mensaje = "El ID del propietario no es válido." });
                }

                var contrato = await _context.Contrato
                    .Where(c => c.Id == idContrato && c.Inmu.IdPropietario == parsedUserId)
                    .FirstOrDefaultAsync();

                if (contrato == null)
                {
                    return NotFound(new { mensaje = "Contrato no pertenece al usuario autenticado." });
                }

                var pagos = await _context.Pago
                    .Where(p => p.IdContrato == idContrato)
                    .Include(p => p.Concepto)
                    .ToListAsync();


                if (pagos == null || pagos.Count == 0)
                {
                    return NotFound(new { mensaje = "No se encontraron pagos para el contrato especificado." });
                }

                return Ok(pagos);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al obtener los pagos: {ex.Message}");
            }
        }



    }
}