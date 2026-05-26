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

        // Elimina la cascada en Reservas para evitar múltiples rutas de borrado
        modelBuilder.Entity<Reserva>()
            .HasOne(r => r.Viaje)
            .WithMany(v => v.Reservas)
            .HasForeignKey(r => r.ViajeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Reserva>()
            .HasOne(r => r.Pasajero)
            .WithMany()
            .HasForeignKey(r => r.PasajeroId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}