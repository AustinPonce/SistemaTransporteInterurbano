using BCrypt.Net;
using System.Linq;
using System.Threading.Tasks;
using SistemaTransporteInterurbano.Models.Entities;

namespace SistemaTransporteInterurbano.DA.Context;

public static class DbInitializer
{
    public static async Task InitializeAsync(AppDbContext context)
    {
        if (!context.Roles.Any())
        {
            context.Roles.AddRange(
                new Rol { RolId = 1, Nombre = "Administrador" },
                new Rol { RolId = 2, Nombre = "Chofer" },
                new Rol { RolId = 3, Nombre = "Pasajero" }
            );

            await context.SaveChangesAsync();
        }

        var admin = context.Usuarios.FirstOrDefault(u => u.NombreUsuario == "admin");
        if (admin == null)
        {
            admin = new Usuario
            {
                NombreUsuario = "admin",
                Clave = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                CorreoElectronico = "admin@sistema.com",
                RolId = 1,
                IntentosFallidos = 0,
                EstaBloqueado = false,
                FechaCreacion = DateTime.Now,
                DebeCambiarClave = false
            };

            context.Usuarios.Add(admin);
        }
        else
        {
            admin.Clave = BCrypt.Net.BCrypt.HashPassword("Admin123!");
            admin.IntentosFallidos = 0;
            admin.EstaBloqueado = false;
        }

        await context.SaveChangesAsync();
    }
}
