using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ImportacionesSusu.Models
{
    public class VentaViewModel
    {
        [Required(ErrorMessage = "Selecciona un cliente")]
        [Display(Name = "Cliente")]
        public int ClienteId { get; set; }
        
        public List<DetalleVentaViewModel> Detalles { get; set; } = new List<DetalleVentaViewModel>();
        
        [Display(Name = "Total General")]
        public decimal TotalGeneral { get; set; }
    }

    public class DetalleVentaViewModel
    {
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; }
        public int Cantidad { get; set; } = 1;
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}