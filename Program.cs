using Microsoft.EntityFrameworkCore;
using ImportacionesSusu.Data;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// CONEXIÓN DIRECTA A NEON - USA TU URL
var connectionString = "Host=ep-cool-dream-a4aodh93-pooler.us-east-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=npg_x3hdJGIrwE2y;SSL Mode=Require;Trust Server Certificate=true;";

Console.WriteLine("🚀 Conectando a Neon PostgreSQL...");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Servicios
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Configuración pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Migración automática
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        Console.WriteLine("🔧 Aplicando migraciones PostgreSQL...");
        db.Database.Migrate();
        Console.WriteLine("✅ Migraciones aplicadas correctamente");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error en migraciones: {ex.Message}");
    }
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
