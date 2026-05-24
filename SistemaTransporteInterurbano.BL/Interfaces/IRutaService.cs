using SistemaTransporteInterurbano.Models.Entities;

namespace SistemaTransporteInterurbano.BL.Interfaces;

public interface IRutaService
{
    Task<List<Ruta>> ObtenerTodasAsync(string? filtro = null);
    Task AgregarAsync(string nombre, string origen, string destino, TimeSpan duracion, decimal precioBase);
    Task<Ruta?> ObtenerPorIdAsync(int id);
    Task EditarAsync(int id, string nombre, string origen, string destino, TimeSpan duracion, decimal precioBase);
}