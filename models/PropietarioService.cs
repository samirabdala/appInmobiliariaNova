using inmobiliaria_AT.Models;
using Microsoft.Extensions.Configuration;

public class PropietarioService : IPropietarioService 
{
    private readonly AppDbContext _context; 
    private readonly IConfiguration _configuration; 
    public PropietarioService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration; 
    }

 public Propietario ValidarPropietario(string email, string Password)
{

    var propietario = _context.Propietario.FirstOrDefault(p => p.Email == email);
    if (propietario == null)
    {
        return null; 
    }


    if (!BCrypt.Net.BCrypt.Verify(Password, propietario.Password))
    {
        return null;
    }

    propietario.Password = null;
    
    return propietario; 
}

}

