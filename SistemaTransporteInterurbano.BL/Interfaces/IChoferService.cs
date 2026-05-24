using SistemaTransporteInterurbano.Models.Entities;

namespace SistemaTransporteInterurbano.BL.Interfaces;

public interface IChoferService
{
    Task<List<Chofer>> ObtenerTodosAsync(string? filtroNombre = null);
    Task AgregarAsync(string identificacion, string nombre, string apellidos, string correo);
    Task<Chofer?> ObtenerPorIdAsync(int id);
    Task EditarAsync(int id, string identificacion, string nombre, string apellidos);
}