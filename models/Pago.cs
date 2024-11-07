using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace inmobiliaria_AT.Models;

public class Pago
{
    [Key]
    public int Id { get; set; }
    public int IdContrato { get; set; }
    public DateTime Fecha { get; set; }
    public double Monto { get; set; }
    public bool Estado { get; set; }
    public DateTime? FechaAnulacion { get; set; }
    public string Detalle { get; set; }
    public int Nro { get; set; }

    public int ConceptoId { get; set; }

    //[ForeignKey("ConceptoId")]
    public Concepto Concepto { get; set; }

}
