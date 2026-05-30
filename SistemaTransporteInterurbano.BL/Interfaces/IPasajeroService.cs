using SistemaTransporteInterurbano.Models.Entities;

namespace SistemaTransporteInterurbano.BL.Interfaces;

public interface IPasajeroService
{
    Task<List<Pasajero>> ObtenerTodosAsync(string? filtroNombre = null);
    Task AgregarAsync(string identificacion, string nombre, string apellidos, string correo);
    Task<Pasajero?> ObtenerPorIdAsync(int id);
    Task EditarAsync(int id, string identificacion, string nombre, string apellidos);
    Task<Pasajero?> ObtenerPorUsuarioIdAsync(int usuarioId);
}