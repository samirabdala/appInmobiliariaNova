using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace inmobiliaria_AT.Models;

public enum Uso
{
    Comercial = 1,
    Residencial = 2
}
public class Inmueble
{
    [Key]
    public int Id { get; set; }

    public Uso Uso { get; set; }

    public string Direccion { get; set; } = "";

    public int TipoId { get; set; } 
    
    [ForeignKey("TipoId")]
    public Tipo Tipo { get; set; } 

    public int Ambientes { get; set; }
    public decimal Latitud { get; set; }
    public decimal Longitud { get; set; }
    public decimal Superficie { get; set; }
    public decimal Precio { get; set; }
    public int IdPropietario { get; set; }
    public bool Estado { get; set; } = true;
    public string ImgUrl { get; set; } = "";
    public ICollection<Contrato> Contratos { get; set; } = new List<Contrato>();

}