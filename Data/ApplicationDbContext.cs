// Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using ImportacionesSusu.Models;
 // Asegúrate de importar tu carpeta Models
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata; // Necesario para UseIdentityColumns

namespace ImportacionesSusu.Data;
public class ApplicationDbContext : DbContext 
{

    public ApplicationDbContext() : base() { }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // AÑADE ESTA PROPIEDAD DbSet para tu tabla Usuario
    public DbSet<Usuario> Usuarios { get; set; } 

    public DbSet<Producto> Productos { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Línea crítica para la generación de IDs en PostgreSQL (simula IDENTITY).
        builder.UseIdentityColumns();
    }
}