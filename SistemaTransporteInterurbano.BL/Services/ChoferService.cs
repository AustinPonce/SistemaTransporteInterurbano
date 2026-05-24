using Microsoft.EntityFrameworkCore;
using SistemaTransporteInterurbano.BL.Helpers;
using SistemaTransporteInterurbano.BL.Interfaces;
using SistemaTransporteInterurbano.DA.Context;
using SistemaTransporteInterurbano.Models.Entities;

namespace SistemaTransporteInterurbano.BL.Services;

public class ChoferService : IChoferService
{
    private readonly AppDbContext _context;
    private readonly INotificacionCorreoService _correoService;

    public ChoferService(AppDbContext context, INotificacionCorreoService correoService)
    {
        _context = context;
        _correoService = correoService;
    }

    public async Task<List<Chofer>> ObtenerTodosAsync(string? filtroNombre = null)
    {
        var choferes = await _context.Choferes
            .OrderBy(c => c.Nombre)
            .ToListAsync();

        if (!string.IsNullOrWhiteSpace(filtroNombre))
            choferes = choferes
                .Where(c =>
                    NormalizadorTextoHelper.Contiene(c.Nombre, filtroNombre) ||
                    NormalizadorTextoHelper.Contiene(c.Apellidos, filtroNombre))
                .ToList();

        return choferes;
    }

    public async Task AgregarAsync(string identificacion, string nombre, string apellidos, string correo)
    {
        var identificacionExiste = await _context.Choferes.AnyAsync(c => c.Identificacion == identificacion);

        if (identificacionExiste)
            throw new Exception("Ya existe un chofer con esa identificación.");

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
            RolId = 2,
            IntentosFallidos = 0,
            EstaBloqueado = false,
            FechaCreacion = DateTime.Now,
            DebeCambiarClave = true
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        var chofer = new Chofer
        {
            Identificacion = identificacion,
            Nombre = nombre,
            Apellidos = apellidos,
            CorreoElectronico = correo,
            UsuarioId = usuario.UsuarioId
        };

        _context.Choferes.Add(chofer);
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

    public async Task<Chofer?> ObtenerPorIdAsync(int id)
    {
        return await _context.Choferes
            .Include(c => c.Usuario)
            .FirstOrDefaultAsync(c => c.ChoferId == id);
    }

    public async Task EditarAsync(int id, string identificacion, string nombre, string apellidos)
    {
        var chofer = await _context.Choferes.FirstOrDefaultAsync(c => c.ChoferId == id);

        if (chofer == null)
            throw new Exception("Chofer no encontrado.");

        var identificacionDuplicada = await _context.Choferes
            .AnyAsync(c => c.Identificacion == identificacion && c.ChoferId != id);

        if (identificacionDuplicada)
            throw new Exception("Ya existe otro chofer con esa identificación.");

        chofer.Identificacion = identificacion;
        chofer.Nombre = nombre;
        chofer.Apellidos = apellidos;

        await _context.SaveChangesAsync();
    }
}