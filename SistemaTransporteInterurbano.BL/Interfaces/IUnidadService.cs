using SistemaTransporteInterurbano.Models.Entities;

namespace SistemaTransporteInterurbano.BL.Interfaces;

public interface IUnidadService
{
    Task<List<Unidad>> ObtenerTodasAsync();
    Task AgregarAsync(string placa, string modelo, int anio, int capacidad);
    Task<Unidad?> ObtenerPorIdAsync(int id);
    Task EditarAsync(int id, string placa, string modelo, int anio, int capacidad);
}