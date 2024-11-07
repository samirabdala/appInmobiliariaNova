
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using inmobiliaria_AT.Models;
using Microsoft.EntityFrameworkCore;

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
                var contrato = await _context.Contrato.FindAsync(idContrato);
                if (contrato == null)
                {
                    return NotFound(new { mensaje = "Contrato no encontrado" });
                }

                var pagos = await _context.Pago
                    .Where(p => p.IdContrato == idContrato)
                    .Include(p => p.Concepto)
                    .ToListAsync();

                if (pagos == null || pagos.Count == 0)
                {
                    return NotFound(new { mensaje = "No se encontraron pagos para el contrato especificado" });
                }

                return Ok(pagos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }


    }
}