// Models/LoginViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace ImportacionesSusu.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido.")]
        public string? Correo { get; set; }

        [Required(ErrorMessage = "La clave es obligatoria.")]
        [DataType(DataType.Password)]
        public string? Clave { get; set; }

        [Display(Name = "¿Recordarme?")]
        public bool RecordarMe { get; set; }
    }
}