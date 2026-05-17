using BCrypt.Net;
using SistemaTransporteInterurbano.BL.Helpers;
using SistemaTransporteInterurbano.BL.Interfaces;
using SistemaTransporteInterurbano.DA.Context;
using SistemaTransporteInterurbano.Models.Entities;

namespace SistemaTransporteInterurbano.BL.Services;

public class UsuarioService : IUsuarioService
{
    private readonly AppDbContext _context;

    private readonly INotificacionCorreoService
        _notificacionCorreoService;

    public UsuarioService(
        AppDbContext context,
        INotificacionCorreoService
            notificacionCorreoService)
    {
        _context = context;

        _notificacionCorreoService =
            notificacionCorreoService;
    }

    public async Task RegistrarUsuarioChofer(
        string nombreUsuario,
        string correoElectronico,
        int rolId)
    {
        var claveTemporal =
            GeneradorClaveHelper
                .GenerarClaveTemporal();

        var claveHash =
            BCrypt.Net.BCrypt
                .HashPassword(claveTemporal);

        var usuario = new Usuario
        {
            NombreUsuario = nombreUsuario,

            CorreoElectronico =
                correoElectronico,

            Clave = claveHash,

            RolId = rolId,

            IntentosFallidos = 0,

            EstaBloqueado = false,

            FechaCreacion = DateTime.Now,

            DebeCambiarClave = true
        };

        _context.Usuarios.Add(usuario);

        await _context.SaveChangesAsync();

        await _notificacionCorreoService
            .EnviarCorreoAsync(
                correoElectronico,
                "Cuenta creada",
                $"Su cuenta fue creada correctamente.\n\n" +
                $"Usuario: {nombreUsuario}\n" +
                $"Clave temporal: {claveTemporal}\n\n" +
                $"Debe cambiar la contraseña al iniciar sesión.");
    }
}