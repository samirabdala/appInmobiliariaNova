using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace inmobiliaria_AT.Models
{
    public class Usuario :IdentityUser
    {
        [Key] 
        public int Id { get; set; } 

        public string usuario { get; set; } = "";
        public string contraseña { get; set; } = "";
    }
}
