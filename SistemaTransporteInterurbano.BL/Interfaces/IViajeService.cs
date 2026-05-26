// BL/Interfaces/IViajeService.cs
using SistemaTransporteInterurbano.Models.Entities;

namespace SistemaTransporteInterurbano.BL.Interfaces
{
    public interface IViajeService
    {
        // ── Módulo 6 ──────────────────────────────────────────
        Task<List<Viaje>> ListarViajesAsync(string? filtroRuta = null, DateTime? filtroFecha = null);
        Task<(bool exito, string mensaje)> AgregarViajeAsync(Viaje viaje);
        Task<(bool exito, string mensaje)> EditarViajeAsync(Viaje viaje);
        Task<(bool exito, string mensaje)> CancelarViajeAsync(int viajeId, string motivo);
        Task<(bool exito, string mensaje)> IniciarViajeAsync(int viajeId);
        Task<Viaje?> ObtenerPorIdAsync(int viajeId);

        // ── Módulo 7 ──────────────────────────────────────────
        Task<List<Viaje>> ObtenerActivosAsync();
        Task ReservarAsientoAsync(int viajeId, int pasajeroId, int asiento);
        Task<List<Reserva>> ObtenerPasajerosAsync(int viajeId);
        Task CancelarReservaAsync(int reservaId);
        Task FinalizarViajeAsync(int viajeId);
        Task<(int pasajeros, int disponibles, decimal total)> ObtenerTotalesAsync(int viajeId);

        // ── Módulo 9 ──────────────────────────────────────────
        Task<List<Reserva>> ObtenerReservasPasajeroAsync(int pasajeroId);
        Task<Viaje?> ObtenerDetalleAsync(int viajeId);
    }
}