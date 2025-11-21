// Controllers/AccountController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity; // Necesario para PasswordHasher
using Microsoft.AspNetCore.Authentication; // Necesario para SignIn
using Microsoft.AspNetCore.Authentication.Cookies; // Necesario para Cookies
using System.Security.Claims; // Necesario para ClaimsIdentity

using ImportacionesSusu.Data;
using ImportacionesSusu.Models; // Usamos Usuario y ViewModels

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;

    public AccountController(ApplicationDbContext context)
    {
        _context = context;
    }

    // ===============================================
    // ACCIÓN DE REGISTRO (GET) - DESHABILITADO
    // ===============================================
    public IActionResult Registro()
    {
        // Redirigir al login, el registro está deshabilitado
        return RedirectToAction("Login");
    }

    // ===============================================
    // ACCIÓN DE REGISTRO (POST) - DESHABILITADO
    // ===============================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Registro(RegistroViewModel model)
    {
        // El registro está deshabilitado, siempre redirigir al login
        return RedirectToAction("Login");
    }
    
    // ===============================================
    // ACCIÓN DE LOGIN (GET)
    // ===============================================
    public IActionResult Login()
    {
        return View();
    }

    // ===============================================
    // ACCIÓN DE LOGIN (POST) - SOLO ADMINISTRADOR
    // ===============================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            // CREDENCIALES FIJAS DEL ADMINISTRADOR - CAMBIA ESTAS!
            var adminEmail = "betyromerohilario@gmail.com";
            var adminPassword = "pedrito"; // ⚠️ CAMBIA ESTA CONTRASEÑA

            // SOLO PERMITIR LOGIN CON LAS CREDENCIALES DEL ADMIN
            if (model.Correo == adminEmail && model.Clave == adminPassword)
            {
                // Crear claims del administrador
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, "1"),
                    new Claim(ClaimTypes.Name, "Administrador"),
                    new Claim(ClaimTypes.Email, adminEmail),
                    new Claim(ClaimTypes.Role, "Administrador") // Rol importante
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RecordarMe,
                    ExpiresUtc = model.RecordarMe ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddMinutes(30)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);
                
                return RedirectToAction("Index", "Producto");
            }
            
            ModelState.AddModelError(string.Empty, "Credenciales inválidas. Solo el administrador puede acceder al sistema.");
        }
        return View(model);
    }

    // ===============================================
    // ACCIÓN DE LOGOUT (Cerrar Sesión)
    // ===============================================
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
}