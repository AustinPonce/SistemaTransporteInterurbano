using System.Linq;
using System.Threading.Tasks;
using SistemaTransporteInterurbano.Models.Entities;

namespace SistemaTransporteInterurbano.DA.Context;

public static class DbInitializer
{
    public static async Task InitializeAsync(AppDbContext context)
    {
        // Si la tabla Roles no existe, Any() lanzará una excepción; el llamador debe manejarlo.
        if (!context.Roles.Any())
        {
            context.Roles.AddRange(
                new Rol { RolId = 1, Nombre = "Administrador" },
                new Rol { RolId = 2, Nombre = "Chofer" },
                new Rol { RolId = 3, Nombre = "Pasajero" }
            );

            await context.SaveChangesAsync();
        }
    }
}
