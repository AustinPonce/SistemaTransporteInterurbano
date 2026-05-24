using SistemaTransporteInterurbano.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaTransporteInterurbano.BL.Interfaces
{
    public interface IViajeService
    {
        Task<List<Viaje>> ObtenerActivosAsync();
        Task ReservarAsientoAsync(int viajeId, int pasajeroId, int asiento);
        Task<List<Reserva>> ObtenerPasajerosAsync(int viajeId);
        Task CancelarReservaAsync(int reservaId);
        Task FinalizarViajeAsync(int viajeId);
        Task<(int pasajeros, int disponibles, decimal total)> ObtenerTotalesAsync(int viajeId);
        Task<List<Reserva>> ObtenerReservasPasajeroAsync(int pasajeroId);
        Task<Viaje?> ObtenerDetalleAsync(int viajeId);
    }
}
