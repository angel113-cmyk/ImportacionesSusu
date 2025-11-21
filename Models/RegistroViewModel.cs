// Models/RegistroViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace ImportacionesSusu.Models
{
    public class RegistroViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string? Nombre { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido.")]
        public string? Correo { get; set; }

        [Required(ErrorMessage = "La clave es obligatoria.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La clave debe tener al menos 6 caracteres.")]
        public string? Clave { get; set; }

        [DataType(DataType.Password)]
        [Compare("Clave", ErrorMessage = "La clave y la confirmación no coinciden.")]
        public string? ConfirmarClave { get; set; }
    }
}