// Models/Producto.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace ImportacionesSusu.Models
{
    public class Producto
    {
        public int Id { get; set; }
        
        [Required]
        public string Codigo { get; set; }
        
        public string? Imagen { get; set; }
        
        [Required]
        public string Nombre { get; set; }
        
        public decimal PrecioCompra { get; set; }
        
        public decimal PrecioVenta { get; set; }
        
        public int Stock { get; set; }
        
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    }
}