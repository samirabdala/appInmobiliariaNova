using inmobiliaria_AT.Models;

public interface IPropietarioService
{
    // Método para validar un propietario mediante su email y contraseña
    Propietario ValidarPropietario(string email, string contraseña);
}
