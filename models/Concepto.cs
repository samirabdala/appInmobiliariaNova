using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace inmobiliaria_AT.Models;

public class Concepto
{
    [Key]
    public int Id { get; set; }
    public String Nombre { get; set; }

     public ICollection<Pago> Pagos{ get; set; } = new List<Pago>();


}