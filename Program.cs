using Microsoft.EntityFrameworkCore;
using ImportacionesSusu.Data;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// CONFIGURACIÓN BASE DE DATOS - POSTGRESQL SIEMPRE
string connectionString;

// PRIORIDAD 1: DATABASE_URL de Render (PRODUCCIÓN - Supabase)
var renderDatabaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
if (!string.IsNullOrEmpty(renderDatabaseUrl))
{
    Console.WriteLine("🚀 PRODUCCIÓN: Conectando a Supabase PostgreSQL");
    connectionString = ConvertDatabaseUrlToConnectionString(renderDatabaseUrl);
}
// PRIORIDAD 2: ConnectionString para desarrollo local
else 
{
    var devConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (!string.IsNullOrEmpty(devConnectionString))
    {
        Console.WriteLine("💻 DESARROLLO: Usando PostgreSQL local");
        connectionString = devConnectionString;
    }
    else
    {
        // Fallback
        Console.WriteLine("⚠️  Usando PostgreSQL de desarrollo por defecto");
        connectionString = "Host=localhost;Database=importacionesSusu;Username=postgres;Password=postgres";
    }
}

Console.WriteLine($"🔗 Cadena conexión: {connectionString?.Split(';')[0]}...");

// CONFIGURAR DbContext - SIEMPRE PostgreSQL
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
        Console.WriteLine("🔧 Aplicando migraciones...");
        db.Database.Migrate();
        Console.WriteLine("✅ Migraciones aplicadas correctamente");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error en migraciones: {ex.Message}");
        Console.WriteLine($"🔍 Detalles: {ex.InnerException?.Message}");
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
            message = "✅ La aplicación está funcionando",
            databaseType = "PostgreSQL"
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

