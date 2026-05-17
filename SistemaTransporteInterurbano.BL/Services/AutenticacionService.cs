using Microsoft.EntityFrameworkCore;
using SistemaTransporteInterurbano.BL.Interfaces;
using SistemaTransporteInterurbano.DA.Context;
using SistemaTransporteInterurbano.Models.Entities;

namespace SistemaTransporteInterurbano.BL.Services;

public class AutenticacionService
    : IAutenticacionService
{
    private readonly AppDbContext _context;

    private readonly INotificacionCorreoService
        _notificacionCorreoService;

    public AutenticacionService(
        AppDbContext context,
        INotificacionCorreoService
            notificacionCorreoService)
    {
        _context = context;

        _notificacionCorreoService =
            notificacionCorreoService;
    }

    public async Task<Usuario?>
        AutenticarUsuarioPorNombreYClave(
            string nombreUsuario,
            string clave)
    {
        var usuario = await _context.Usuarios
            .Include(x => x.Rol)
            .FirstOrDefaultAsync(x =>
                x.NombreUsuario == nombreUsuario);

        if (usuario == null)
        {
            throw new Exception(
                "El usuario no existe.");
        }

        if (usuario.Rol!.Nombre != "Administrador")
        {
            if (usuario.EstaBloqueado &&
                usuario.FechaBloqueo > DateTime.Now)
            {
                await
                    _notificacionCorreoService
                    .EnviarCorreoAsync(
                        usuario.CorreoElectronico,
                        "Cuenta bloqueada",
                        $"La cuenta {usuario.NombreUsuario} está bloqueada por 3 minutos.");

                throw new Exception(
                    "La cuenta está bloqueada temporalmente.");
            }
        }

        var claveCorrecta =
            BCrypt.Net.BCrypt.Verify(
                clave,
                usuario.Clave);

        if (!claveCorrecta)
        {
            if (usuario.Rol!.Nombre != "Administrador")
            {
                usuario.IntentosFallidos++;

                if (usuario.IntentosFallidos >= 2)
                {
                    usuario.EstaBloqueado = true;

                    usuario.FechaBloqueo =
                        DateTime.Now.AddMinutes(3);

                    await
                        _notificacionCorreoService
                        .EnviarCorreoAsync(
                            usuario.CorreoElectronico,
                            "Cuenta bloqueada",
                            $"La cuenta {usuario.NombreUsuario} está bloqueada por 3 minutos.");
                }

                await _context.SaveChangesAsync();
            }

            throw new Exception(
                "La clave es incorrecta.");
        }

        usuario.IntentosFallidos = 0;

        usuario.EstaBloqueado = false;

        await _context.SaveChangesAsync();

        await _notificacionCorreoService
            .EnviarCorreoAsync(
                usuario.CorreoElectronico,
                $"Inicio de sesión — {usuario.NombreUsuario}",
                $"Usted inició sesión el día {DateTime.Now:dd/MM/yyyy} a las {DateTime.Now:HH:mm}");

        if (usuario.DebeCambiarClave)
        {
            throw new Exception(
                "Debe cambiar la contraseña antes de continuar.");
        }

        return usuario;
    }

    public async Task CambiarClaveDeUsuario(
        string nombreUsuario,
        string claveActual,
        string nuevaClave)
    {
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(x =>
                x.NombreUsuario == nombreUsuario);

        if (usuario == null)
        {
            throw new Exception(
                "El usuario no existe.");
        }

        var claveActualCorrecta =
            BCrypt.Net.BCrypt.Verify(
                claveActual,
                usuario.Clave);

        if (!claveActualCorrecta)
        {
            throw new Exception(
                "La clave actual es incorrecta.");
        }

        usuario.Clave =
            BCrypt.Net.BCrypt
                .HashPassword(nuevaClave);

        usuario.DebeCambiarClave = false;

        await _context.SaveChangesAsync();

        await _notificacionCorreoService
            .EnviarCorreoAsync(
                usuario.CorreoElectronico,
                $"Cambio de clave — {usuario.NombreUsuario}",
                $"La clave fue actualizada el día {DateTime.Now:dd/MM/yyyy} a las {DateTime.Now:HH:mm}");
    }
}

