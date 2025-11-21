// Controllers/ClienteController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ImportacionesSusu.Data;
using ImportacionesSusu.Models;

namespace ImportacionesSusu.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ClienteController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClienteController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Lista todos los clientes
        public async Task<IActionResult> Index(string searchString)
        {
            var clientes = from c in _context.Clientes select c;

            if (!string.IsNullOrEmpty(searchString))
            {
                clientes = clientes.Where(c => 
                    c.Nombre.Contains(searchString) || 
                    c.NumeroCliente.Contains(searchString) ||
                    c.Telefono.Contains(searchString));
            }

            clientes = clientes.OrderByDescending(c => c.FechaRegistro);
            
            return View(await clientes.ToListAsync());
        }

        // GET: Formulario para crear
        public IActionResult Create()
        {
            return View();
        }

        // POST: Crear nuevo cliente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cliente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        // GET: Formulario para editar
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();
            return View(cliente);
        }

        // POST: Actualizar cliente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Cliente cliente)
        {
            if (id != cliente.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cliente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(cliente.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        // GET: Detalles del cliente
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var cliente = await _context.Clientes.FirstOrDefaultAsync(m => m.Id == id);
            if (cliente == null) return NotFound();
            return View(cliente);
        }

        // GET: Confirmar eliminación
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var cliente = await _context.Clientes.FirstOrDefaultAsync(m => m.Id == id);
            if (cliente == null) return NotFound();
            return View(cliente);
        }

        // POST: Eliminar cliente
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente != null)
            {
                _context.Clientes.Remove(cliente);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // EXPORTAR a CSV
        public async Task<IActionResult> ExportarCSV()
        {
            var clientes = await _context.Clientes.ToListAsync();
            
            var csv = new System.Text.StringBuilder();
            csv.AppendLine("NumeroCliente,Nombre,Telefono,FechaRegistro");
            
            foreach (var cliente in clientes)
            {
                csv.AppendLine($"\"{cliente.NumeroCliente}\",\"{cliente.Nombre}\",\"{cliente.Telefono}\",\"{cliente.FechaRegistro:yyyy-MM-dd}\"");
            }
            
            return File(System.Text.Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", "clientes.csv");
        }

        private bool ClienteExists(int id)
        {
            return _context.Clientes.Any(e => e.Id == id);
        }
    }
}