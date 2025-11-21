// Models/Usuario.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImportacionesSusu.Models
{
    // Opcional: Especifica el nombre exacto de la tabla.
    [Table("Usuario")] 
    public class Usuario
    {
        // IdUsuario: La clave primaria auto-incremental.
        // PostgreSQL maneja el 'identity' automáticamente con 'int primary key'.
        [Key]
        public int IdUsuario { get; set; } 

        [Required]
        [StringLength(50)]
        public string? Nombre { get; set; }

        [Required]
        [StringLength(50)]
        public string? Correo { get; set; }

        // Clave: Debe ser un hash, por eso el largo (200).
        [Required]
        [StringLength(200)]
        public string? Clave { get; set; }

        public string? ConfirmarClave { get; set; }
        // Restablecer y Confirmado: Los tipos 'bit' en SQL se mapean a 'bool' en C#.
        public bool Restablecer { get; set; }

        public bool Confirmado { get; set; }

        [StringLength(50)]
        public string? Token { get; set; }
    }
}