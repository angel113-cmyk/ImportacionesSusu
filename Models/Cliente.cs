// Models/Cliente.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace ImportacionesSusu.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El número de cliente es obligatorio")]
        [Display(Name = "Número de Cliente")]
        public string NumeroCliente { get; set; }
        
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }
        
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }
        
        [Display(Name = "Fecha de Registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    }
}