using System.ComponentModel.DataAnnotations;

namespace ImportacionesSusu.Models
{
    public class DetalleVenta
    {
        public int Id { get; set; }
        
        public int VentaId { get; set; }
        public Venta Venta { get; set; }
        
        [Required]
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        [Display(Name = "Precio Unitario")]
        public decimal PrecioUnitario { get; set; }
        
        [Display(Name = "Subtotal")]
        public decimal Subtotal { get; set; }
    }
}