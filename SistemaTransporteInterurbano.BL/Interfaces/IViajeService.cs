using SistemaTransporteInterurbano.Models.Entities;

namespace SistemaTransporteInterurbano.BL.Interfaces
{
    public interface IViajeService
    {
        // ── MÓDULO 6 — Gestión de Viajes ──────────────────────────────
        Task<List<Viaje>> ObtenerTodosAsync(string? filtroRuta, DateTime? filtroFecha);
        Task<Viaje?> ObtenerPorIdAsync(int id);
        Task AgregarAsync(int rutaId, int unidadId, int choferId, DateTime fechaSalida, DateTime fechaLlegada);
        Task EditarAsync(int id, int rutaId, int unidadId, int choferId, DateTime fechaSalida, DateTime fechaLlegada);
        Task CancelarAsync(int id, string motivo, INotificacionCorreoService correoService);
        Task IniciarAsync(int id);

        // ── MÓDULO 7 — Viajes en Curso ────────────────────────────────
        Task<List<Viaje>> ObtenerActivosAsync();
        Task ReservarAsientoAsync(int viajeId, int pasajeroId, int asiento);
        Task<List<Reserva>> ObtenerPasajerosAsync(int viajeId);
        Task CancelarReservaAsync(int reservaId);
        Task FinalizarViajeAsync(int viajeId);
        Task<(int pasajeros, int disponibles, decimal total)> ObtenerTotalesAsync(int viajeId);

        // ── MÓDULO 8 — Viajes Cancelados ─────────────────────────────
        Task<List<Viaje>> ObtenerCanceladosAsync();

        // ── MÓDULO 9 — Mis Viajes (Pasajero) ─────────────────────────
        Task<List<Reserva>> ObtenerReservasPasajeroAsync(int pasajeroId);
        Task<Viaje?> ObtenerDetalleAsync(int viajeId);
    }
}
