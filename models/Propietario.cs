using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
namespace inmobiliaria_AT.Models;

public class Propietario
{
    [Key]
    public int Id { get; set; }
    public String Nombre { get; set; } = "";
    public String Apellido { get; set; } = "";
    public String Documento { get; set; } = "";
    public String Telefono { get; set; } = "";
    [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
    public String Email { get; set; } = "";
    public String Direccion { get; set; } = "";
    public String Avatar { get; set; } = "";

    public String Password { get; set; } = "";
    public String NombreCompleto => $"{Nombre} {Apellido}";
    public ICollection<Contrato> Contratos { get; set; } = new List<Contrato>();

}