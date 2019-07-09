using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.Dtos
{
    public class UserForRegisterDto
    {
       [Required(ErrorMessage = "Campo requerido")]
       public string Username { get; set; } 

       [Required(ErrorMessage = "Campo requerido")]
       [StringLength(20, MinimumLength = 6, ErrorMessage = "Usted debe espeficar un Password que contenga un mínimo de 6 caracteres")]
       public string Password { get; set; }
    }
}