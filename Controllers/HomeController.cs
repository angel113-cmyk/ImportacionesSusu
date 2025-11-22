// Controllers/HomeController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ImportacionesSusu.Data;
using ImportacionesSusu.Models;

namespace ImportacionesSusu.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var dashboard = new DashboardViewModel
            {
                TotalProductos = await _context.Productos.CountAsync(),
                TotalClientes = await _context.Clientes.CountAsync(),
                ProductosStockBajo = await _context.Productos.CountAsync(p => p.Stock <= 5),
                ValorTotalInventario = await _context.Productos.SumAsync(p => p.Stock * p.PrecioCompra),
                ProductosRecientes = await _context.Productos
                    .OrderByDescending(p => p.FechaRegistro)
                    .Take(5)
                    .ToListAsync(),
                ClientesRecientes = await _context.Clientes
                    .OrderByDescending(c => c.FechaRegistro)
                    .Take(5)
                    .ToListAsync()
            };

            return View(dashboard);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}