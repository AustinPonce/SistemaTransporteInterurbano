using Microsoft.EntityFrameworkCore;
using SistemaTransporteInterurbano.BL.Helpers;
using SistemaTransporteInterurbano.BL.Interfaces;
using SistemaTransporteInterurbano.DA.Context;
using SistemaTransporteInterurbano.Models.Entities;

namespace SistemaTransporteInterurbano.BL.Services;

public class PasajeroService : IPasajeroService
{
    private readonly AppDbContext _context;
    private readonly INotificacionCorreoService _correoService;

    public PasajeroService(AppDbContext context, INotificacionCorreoService correoService)
    {
        _context = context;
        _correoService = correoService;
    }

    public async Task<List<Pasajero>> ObtenerTodosAsync(string? filtroNombre = null)
    {
        var pasajeros = await _context.Pasajeros
            .OrderBy(p => p.Nombre)
            .ToListAsync();

        if (!string.IsNullOrWhiteSpace(filtroNombre))
            pasajeros = pasajeros
                .Where(p =>
                    NormalizadorTextoHelper.Contiene(p.Nombre, filtroNombre) ||
                    NormalizadorTextoHelper.Contiene(p.Apellidos, filtroNombre))
                .ToList();

        return pasajeros;
    }

    public async Task AgregarAsync(string identificacion, string nombre, string apellidos, string correo)
    {
        var identificacionExiste = await _context.Pasajeros.AnyAsync(p => p.Identificacion == identificacion);

        if (identificacionExiste)
            throw new Exception("Ya existe un pasajero con esa identificación.");

        var claveTemporal = GeneradorClaveHelper.GenerarClaveTemporal();
        var claveHash = BCrypt.Net.BCrypt.HashPassword(claveTemporal);

        var nombreUsuario = nombre.ToLower().Replace(" ", "") + apellidos.Split(' ')[0].ToLower();

        var usuarioExiste = await _context.Usuarios.AnyAsync(u => u.NombreUsuario == nombreUsuario);
        if (usuarioExiste)
            nombreUsuario += identificacion;

        var usuario = new Usuario
        {
            NombreUsuario = nombreUsuario,
            CorreoElectronico = correo,
            Clave = claveHash,
            RolId = 3,
            IntentosFallidos = 0,
            EstaBloqueado = false,
            FechaCreacion = DateTime.Now,
            DebeCambiarClave = true
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        var pasajero = new Pasajero
        {
            Identificacion = identificacion,
            Nombre = nombre,
            Apellidos = apellidos,
            CorreoElectronico = correo,
            UsuarioId = usuario.UsuarioId
        };

        _context.Pasajeros.Add(pasajero);
        await _context.SaveChangesAsync();

        try
        {
            await _correoService.EnviarCorreoAsync(
                correo,
                "Cuenta creada — Sistema de Transporte Interurbano",
                $"Bienvenido/a {nombre} {apellidos}.\n\n" +
                $"Se ha creado su cuenta en el sistema.\n\n" +
                $"Usuario: {nombreUsuario}\n" +
                $"Clave temporal: {claveTemporal}\n\n" +
                $"Debe cambiar la contraseña al iniciar sesión por primera vez.");
        }
        catch { }
    }

    public async Task<Pasajero?> ObtenerPorIdAsync(int id)
    {
        return await _context.Pasajeros
            .Include(p => p.Usuario)
            .FirstOrDefaultAsync(p => p.PasajeroId == id);
    }

    public async Task EditarAsync(int id, string identificacion, string nombre, string apellidos)
    {
        var pasajero = await _context.Pasajeros.FirstOrDefaultAsync(p => p.PasajeroId == id);

        if (pasajero == null)
            throw new Exception("Pasajero no encontrado.");

        var identificacionDuplicada = await _context.Pasajeros
            .AnyAsync(p => p.Identificacion == identificacion && p.PasajeroId != id);

        if (identificacionDuplicada)
            throw new Exception("Ya existe otro pasajero con esa identificación.");

        pasajero.Identificacion = identificacion;
        pasajero.Nombre = nombre;
        pasajero.Apellidos = apellidos;

        await _context.SaveChangesAsync();
    }
    public async Task<Pasajero?> ObtenerPorUsuarioIdAsync(int usuarioId)
    {
        return await _context.Pasajeros
            .Include(p => p.Usuario)
            .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId);
    }
}