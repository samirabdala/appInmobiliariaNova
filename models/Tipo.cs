

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;
namespace inmobiliaria_AT.Models;
[Table("tipo")] 

public class Tipo
{
    [Key]
    public int Id { get; set; }
    public string Nombre { get; set; }
    public bool Estado { get; set; }


public ICollection<Inmueble> Inmuebles { get; set; } = new List<Inmueble>();
}
