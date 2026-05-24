using Microsoft.EntityFrameworkCore;
using SistemaTransporteInterurbano.BL.Interfaces;
using SistemaTransporteInterurbano.DA.Context;
using SistemaTransporteInterurbano.Models.Entities;

namespace SistemaTransporteInterurbano.BL.Services;

public class UnidadService : IUnidadService
{
    private readonly AppDbContext _context;

    public UnidadService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Unidad>> ObtenerTodasAsync()
    {
        return await _context.Unidades
            .OrderBy(u => u.Placa)
            .ToListAsync();
    }

    public async Task AgregarAsync(string placa, string modelo, int anio, int capacidad)
    {
        var placaExiste = await _context.Unidades
            .AnyAsync(u => u.Placa == placa);

        if (placaExiste)
            throw new Exception("Ya existe una unidad con esa placa.");

        var unidad = new Unidad
        {
            Placa = placa,
            Modelo = modelo,
            AnioFabricacion = anio,
            CapacidadPasajeros = capacidad
        };

        _context.Unidades.Add(unidad);
        await _context.SaveChangesAsync();
    }

    public async Task<Unidad?> ObtenerPorIdAsync(int id)
    {
        return await _context.Unidades
            .FirstOrDefaultAsync(u => u.UnidadId == id);
    }

    public async Task EditarAsync(int id, string placa, string modelo, int anio, int capacidad)
    {
        var unidad = await _context.Unidades
            .FirstOrDefaultAsync(u => u.UnidadId == id);

        if (unidad == null)
            throw new Exception("Unidad no encontrada.");

        var placaDuplicada = await _context.Unidades
            .AnyAsync(u => u.Placa == placa && u.UnidadId != id);

        if (placaDuplicada)
            throw new Exception("Ya existe otra unidad con esa placa.");

        unidad.Placa = placa;
        unidad.Modelo = modelo;
        unidad.AnioFabricacion = anio;
        unidad.CapacidadPasajeros = capacidad;

        await _context.SaveChangesAsync();
    }
}