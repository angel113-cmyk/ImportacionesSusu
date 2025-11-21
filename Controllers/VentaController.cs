// Controllers/VentaController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ImportacionesSusu.Data;
using ImportacionesSusu.Models;

namespace ImportacionesSusu.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class VentaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VentaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Lista todas las ventas
       
    public async Task<IActionResult> Index()
{
    Console.WriteLine("=== CARGANDO LISTA DE VENTAS ===");
    
    var ventas = await _context.Ventas
        .Include(v => v.Cliente)
        .Include(v => v.Detalles)
            .ThenInclude(d => d.Producto)
        .OrderByDescending(v => v.FechaVenta)
        .ToListAsync();

    Console.WriteLine($"Ventas encontradas: {ventas.Count}");
    
    return View(ventas);
}

        // GET: Formulario para crear venta
        public async Task<IActionResult> Create()
        {
            ViewBag.Clientes = await _context.Clientes.ToListAsync();
            ViewBag.Productos = await _context.Productos.Where(p => p.Stock > 0).ToListAsync();
            
            var model = new VentaViewModel();
            return View(model);
        }

        // POST: Crear nueva venta
        [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(VentaViewModel model)
{
    Console.WriteLine("=== DEBUG VENTA ===");
    Console.WriteLine($"ClienteId: {model.ClienteId}, Total: {model.TotalGeneral}, Detalles: {model.Detalles?.Count}");

    // TEMPORAL: Ignorar ModelState para probar
    if (model.Detalles != null && model.Detalles.Any()) // && ModelState.IsValid
    {
        Console.WriteLine("✅ PROCESANDO VENTA (ignorando ModelState)...");
        
        using var transaction = await _context.Database.BeginTransactionAsync();
        
        try
        {
            var venta = new Venta
            {
                ClienteId = model.ClienteId,
                FechaVenta = DateTime.UtcNow,
                Total = model.TotalGeneral,
                Estado = "Completada"
            };

            _context.Ventas.Add(venta);
            await _context.SaveChangesAsync();

            foreach (var detalleModel in model.Detalles)
            {
                var producto = await _context.Productos.FindAsync(detalleModel.ProductoId);
                
                if (producto != null && producto.Stock >= detalleModel.Cantidad)
                {
                    var detalleVenta = new DetalleVenta
                    {
                        VentaId = venta.Id,
                        ProductoId = detalleModel.ProductoId,
                        Cantidad = detalleModel.Cantidad,
                        PrecioUnitario = detalleModel.PrecioUnitario,
                        Subtotal = detalleModel.Subtotal
                    };

                    _context.DetalleVentas.Add(detalleVenta);
                    
                    producto.Stock -= detalleModel.Cantidad;
                    _context.Productos.Update(producto);
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            
            Console.WriteLine($"🎉 VENTA EXITOSA - ID: {venta.Id}");
            return RedirectToAction(nameof(Details), new { id = venta.Id });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"❌ ERROR: {ex.Message}");
            ModelState.AddModelError("", $"Error: {ex.Message}");
        }
    }

    ViewBag.Clientes = await _context.Clientes.ToListAsync();
    ViewBag.Productos = await _context.Productos.Where(p => p.Stock > 0).ToListAsync();
    return View(model);
}
        // GET: Detalles de la venta
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var venta = await _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Detalles)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venta == null) return NotFound();

            return View(venta);
        }

        // GET: Confirmar eliminación
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var venta = await _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Detalles)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venta == null) return NotFound();

            return View(venta);
        }

        // POST: Eliminar venta (y revertir stock)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                var venta = await _context.Ventas
                    .Include(v => v.Detalles)
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (venta != null)
                {
                    // Revertir stock de productos
                    foreach (var detalle in venta.Detalles)
                    {
                        var producto = await _context.Productos.FindAsync(detalle.ProductoId);
                        if (producto != null)
                        {
                            producto.Stock += detalle.Cantidad;
                            _context.Productos.Update(producto);
                        }
                    }

                    // Eliminar detalles primero
                    _context.DetalleVentas.RemoveRange(venta.Detalles);
                    // Luego eliminar la venta
                    _context.Ventas.Remove(venta);
                    
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // API: Obtener producto para AJAX
        [HttpGet]
        public async Task<IActionResult> GetProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound();

            return Json(new { 
                id = producto.Id,
                nombre = producto.Nombre,
                precioVenta = producto.PrecioVenta,
                stock = producto.Stock
            });
        }

        private bool VentaExists(int id)
        {
            return _context.Ventas.Any(e => e.Id == id);
        }
    }
}