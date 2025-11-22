using Microsoft.EntityFrameworkCore;
using ImportacionesSusu.Data;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// CONFIGURACIÓN BASE DE DATOS MEJORADA
string connectionString;

// PRIORIDAD 1: DATABASE_URL de Render (PRODUCCIÓN)
var renderDatabaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
if (!string.IsNullOrEmpty(renderDatabaseUrl))
{
    Console.WriteLine("🚀 Usando DATABASE_URL de Render (PRODUCCIÓN)");
    connectionString = ConvertDatabaseUrlToConnectionString(renderDatabaseUrl);
}
// PRIORIDAD 2: ConnectionString del appsettings (DESARROLLO)
else 
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    Console.WriteLine("💻 Usando ConnectionString local (DESARROLLO)");
}

Console.WriteLine($"🔗 Base de datos: {connectionString?.Split(';')[0]}...");

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

// Migración automática con mejor manejo de errores
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        Console.WriteLine("🔧 Intentando conectar a la base de datos...");
        
        // Verificar si podemos conectar primero
        if (db.Database.CanConnect())
        {
            Console.WriteLine("✅ Conexión exitosa, aplicando migraciones...");
            db.Database.Migrate();
            Console.WriteLine("✅ Migraciones aplicadas correctamente");
        }
        else
        {
            Console.WriteLine("❌ No se pudo conectar a la base de datos");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error en migraciones: {ex.Message}");
        Console.WriteLine($"🔍 StackTrace: {ex.StackTrace}");
    }
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Endpoint de prueba
app.MapGet("/test-db", async (ApplicationDbContext db) => 
{
    try 
    {
        var canConnect = await db.Database.CanConnectAsync();
        return Results.Ok(new { 
            status = "success", 
            databaseConnected = canConnect,
            message = "✅ La aplicación está funcionando"
        });
    }
    catch (Exception ex)
    {
        return Results.Problem($"❌ Error de base de datos: {ex.Message}");
    }
});

app.Run();

// Función para convertir DATABASE_URL
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

