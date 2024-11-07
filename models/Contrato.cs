namespace inmobiliaria_AT.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Contrato
{
    [Key]
    public int Id { get; set; }


    public Inquilino Inqui { get; set; } = null!;

    public int InquiId { get; set; }

    public Inmueble Inmu { get; set; } = null!;

    public int InmuId { get; set; }

    public Propietario Prop { get; set; } = null!;

    public int PropId { get; set; }
    public DateTime FechaInicio { get; set; }

    public DateTime FechaFin { get; set; }

    public int? Estado { get; set; }

    public string Descripcion { get; set; } = "";

    public string Observaciones { get; set; } = "";


    public int? Pagos { get; set; }

    public void CalcularCantidadPagos()
    {
        var duration = FechaFin - FechaInicio;
        if (duration.Days < 30)
        {
            Pagos = 1; // Pago único
        }
        else
        {
            Pagos = (int)Math.Ceiling(duration.TotalDays / 30); 
        }
    }
    public decimal? PrecioInmueble => Inmu?.Precio ?? 0;

    public string DireccionInmueble => Inmu != null ? Inmu.Direccion : "Dirección no disponible";

}