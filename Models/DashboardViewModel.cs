// Models/DashboardViewModel.cs
namespace ImportacionesSusu.Models
{
    public class DashboardViewModel
    {
        public int TotalProductos { get; set; }
        public int TotalClientes { get; set; }
        public int ProductosStockBajo { get; set; }
        public decimal ValorTotalInventario { get; set; }
        public List<Producto> ProductosRecientes { get; set; } = new();
        public List<Cliente> ClientesRecientes { get; set; } = new();
    }
}