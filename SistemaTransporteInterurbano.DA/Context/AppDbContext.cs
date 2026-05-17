using Microsoft.EntityFrameworkCore;
using SistemaTransporteInterurbano.Models.Entities;

namespace SistemaTransporteInterurbano.DA.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; }

    public DbSet<Rol> Roles { get; set; }

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Rol>()
            .HasData(
                new Rol
                {
                    RolId = 1,
                    Nombre = "Administrador"
                },
                new Rol
                {
                    RolId = 2,
                    Nombre = "Chofer"
                },
                new Rol
                {
                    RolId = 3,
                    Nombre = "Pasajero"
                }
            );

        modelBuilder.Entity<Usuario>()
            .HasData(
                new Usuario
                {
                    UsuarioId = 1,
                    NombreUsuario = "Administrador",
                    Clave = "TicoBus2025*",
                    CorreoElectronico =
                        "transporteinterurbano93@gmail.com",
                    RolId = 1,
                    IntentosFallidos = 0,
                    EstaBloqueado = false,
                    FechaCreacion = new DateTime(2025, 1, 1)
                }
            );
    }
}