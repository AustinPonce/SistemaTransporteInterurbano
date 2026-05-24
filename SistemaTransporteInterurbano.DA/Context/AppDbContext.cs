using Microsoft.EntityFrameworkCore;
using SistemaTransporteInterurbano.Models.Entities;

namespace SistemaTransporteInterurbano.DA.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; }

    public DbSet<Rol> Roles { get; set; }

    public DbSet<PasswordReset> PasswordResets { get; set; }

    public DbSet<Chofer> Choferes { get; set; }

    public DbSet<Pasajero> Pasajeros { get; set; }

    public DbSet<Ruta> Rutas { get; set; }

    public DbSet<Unidad> Unidades { get; set; }

    public DbSet<Viaje> Viajes { get; set; }
    public DbSet<Reserva> Reservas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}