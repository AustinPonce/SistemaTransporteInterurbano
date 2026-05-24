using Microsoft.EntityFrameworkCore;
using SistemaTransporteInterurbano.BL.Helpers;
using SistemaTransporteInterurbano.BL.Interfaces;
using SistemaTransporteInterurbano.DA.Context;
using SistemaTransporteInterurbano.Models.Entities;

namespace SistemaTransporteInterurbano.BL.Services;

public class RutaService : IRutaService
{
    private readonly AppDbContext _context;

    public RutaService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Ruta>> ObtenerTodasAsync(string? filtro = null)
    {
        var rutas = await _context.Rutas
            .OrderBy(r => r.Nombre)
            .ToListAsync();

        if (!string.IsNullOrWhiteSpace(filtro))
            rutas = rutas
                .Where(r =>
                    NormalizadorTextoHelper.Contiene(r.Nombre, filtro) ||
                    NormalizadorTextoHelper.Contiene(r.Destino, filtro))
                .ToList();

        return rutas;
    }

    public async Task AgregarAsync(string nombre, string origen, string destino, TimeSpan duracion, decimal precioBase)
    {
        var ruta = new Ruta
        {
            Nombre = nombre,
            Origen = origen,
            Destino = destino,
            DuracionEstimada = duracion,
            PrecioBase = precioBase
        };

        _context.Rutas.Add(ruta);
        await _context.SaveChangesAsync();
    }

    public async Task<Ruta?> ObtenerPorIdAsync(int id)
    {
        return await _context.Rutas.FirstOrDefaultAsync(r => r.RutaId == id);
    }

    public async Task EditarAsync(int id, string nombre, string origen, string destino, TimeSpan duracion, decimal precioBase)
    {
        var ruta = await _context.Rutas.FirstOrDefaultAsync(r => r.RutaId == id);

        if (ruta == null)
            throw new Exception("Ruta no encontrada.");

        ruta.Nombre = nombre;
        ruta.Origen = origen;
        ruta.Destino = destino;
        ruta.DuracionEstimada = duracion;
        ruta.PrecioBase = precioBase;

        await _context.SaveChangesAsync();
    }
}