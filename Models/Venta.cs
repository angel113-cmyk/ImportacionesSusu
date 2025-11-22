using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ImportacionesSusu.Models
{
    public class Venta
    {
        public int Id { get; set; }
        
        [Required]
        [Display(Name = "Cliente")]
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }
        
        [Display(Name = "Fecha de Venta")]
        public DateTime FechaVenta { get; set; } = DateTime.UtcNow;
        
        [Display(Name = "Total")]
        public decimal Total { get; set; }
        
        [Display(Name = "Estado")]
        public string Estado { get; set; } = "Completada";
        
        public List<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();
    }
}