using Microsoft.EntityFrameworkCore;
using ImportacionesSusu.Data;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Configurar PostgreSQL para Render
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Si no hay connection string, usar DATABASE_URL de Render
if (string.IsNullOrEmpty(connectionString))
{
    var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
    if (!string.IsNullOrEmpty(databaseUrl))
    {
        connectionString = ConvertDatabaseUrlToConnectionString(databaseUrl);
    }
    else
    {
        // Fallback local
        connectionString = "Host=localhost;Database=importacionesSusu;Username=soporteangel;Password=soporte";
    }
}

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

// Migración automática en producción
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
        Console.WriteLine("✅ Migraciones aplicadas correctamente");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error en migraciones: {ex.Message}");
    }
}

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// Función para convertir DATABASE_URL de Render
static string ConvertDatabaseUrlToConnectionString(string databaseUrl)
{
    try
    {
        var uri = new Uri(databaseUrl);
        var db = uri.AbsolutePath.Trim('/');
        var user = uri.UserInfo.Split(':')[0];
        var passwd = uri.UserInfo.Split(':')[1];
        var port = uri.Port > 0 ? uri.Port : 5432;
        var host = uri.Host;
        
        return $"Host={host};Port={port};Database={db};Username={user};Password={passwd};SSL Mode=Require;Trust Server Certificate=true;";
    }
    catch (Exception ex)
    {
        throw new Exception($"Error parsing DATABASE_URL: {ex.Message}");
    }
}