using Microsoft.EntityFrameworkCore;
using SistemaTransporteInterurbano.BL.Interfaces;
using SistemaTransporteInterurbano.DA;
using SistemaTransporteInterurbano.DA.Context;
using SistemaTransporteInterurbano.Models.Entities;

namespace SistemaTransporteInterurbano.BL.Services
{
    public class ViajeService : IViajeService
    {
        private readonly AppDbContext _context;

        public ViajeService(AppDbContext context)
        {
            _context = context;
        }

        // ==========================
        // MODULO 7
        // ==========================

        // Mostrar viajes en curso
        public async Task<List<Viaje>> ObtenerActivosAsync()
        {
            return await _context.Viajes
                .Include(v => v.Ruta)
                .Include(v => v.Unidad)
                .Include(v => v.Chofer)
                .Include(v => v.Reservas)
                .Where(v => v.Estado == EstadoViaje.EnCurso)
                .ToListAsync();
        }

        // Reservar asiento
        public async Task ReservarAsientoAsync(int viajeId, int pasajeroId, int asiento)
        {
            var viaje = await _context.Viajes
                .Include(v => v.Ruta)
                .Include(v => v.Unidad)
                .Include(v => v.Reservas)
                .FirstOrDefaultAsync(v => v.ViajeId == viajeId);

            if (viaje == null)
                throw new Exception("Viaje no encontrado.");

            if (viaje.Estado != EstadoViaje.EnCurso)
                throw new Exception("El viaje no está en curso.");

            // validar asiento ocupado
            bool asientoOcupado = await _context.Reservas
                .AnyAsync(r => r.ViajeId == viajeId &&
                               r.NumeroAsiento == asiento);

            if (asientoOcupado)
                throw new Exception("Ese asiento ya está ocupado.");

            // validar capacidad
            if (viaje.Reservas.Count >= viaje.Unidad.CapacidadPasajeros)
                throw new Exception("Capacidad máxima alcanzada.");

            var reserva = new Reserva
            {
                ViajeId = viajeId,
                PasajeroId = pasajeroId,
                NumeroAsiento = asiento,
                MontoPagado = viaje.Ruta.PrecioBase
            };

            _context.Reservas.Add(reserva);

            await _context.SaveChangesAsync();
        }

        // Ver pasajeros embarcados
        public async Task<List<Reserva>> ObtenerPasajerosAsync(int viajeId)
        {
            return await _context.Reservas
                .Include(r => r.Pasajero)
                .Where(r => r.ViajeId == viajeId)
                .OrderBy(r => r.NumeroAsiento)
                .ToListAsync();
        }

        // Cancelar reserva
        public async Task CancelarReservaAsync(int reservaId)
        {
            var reserva = await _context.Reservas
                .FirstOrDefaultAsync(r => r.ReservaId == reservaId);

            if (reserva == null)
                throw new Exception("Reserva no encontrada.");

            _context.Reservas.Remove(reserva);

            await _context.SaveChangesAsync();
        }

        // Finalizar viaje
        public async Task FinalizarViajeAsync(int viajeId)
        {
            var viaje = await _context.Viajes
                .FirstOrDefaultAsync(v => v.ViajeId == viajeId);

            if (viaje == null)
                throw new Exception("Viaje no encontrado.");

            if (viaje.Estado != EstadoViaje.EnCurso)
                throw new Exception("Solo se pueden finalizar viajes en curso.");

            viaje.Estado = EstadoViaje.Completado;

            await _context.SaveChangesAsync();
        }

        // Totales / recaudación
        public async Task<(int pasajeros, int disponibles, decimal total)>
            ObtenerTotalesAsync(int viajeId)
        {
            var viaje = await _context.Viajes
                .Include(v => v.Unidad)
                .Include(v => v.Reservas)
                .FirstOrDefaultAsync(v => v.ViajeId == viajeId);

            if (viaje == null)
                throw new Exception("Viaje no encontrado.");

            int pasajeros = viaje.Reservas.Count;
            int disponibles = viaje.Unidad.CapacidadPasajeros - pasajeros;
            decimal total = viaje.Reservas.Sum(r => r.MontoPagado);

            return (pasajeros, disponibles, total);
        }


        // ==========================
        // MODULO 9
        // ==========================

        // Mis reservas
        public async Task<List<Reserva>> ObtenerReservasPasajeroAsync(int pasajeroId)
        {
            return await _context.Reservas
                .Include(r => r.Viaje)
                    .ThenInclude(v => v.Ruta)
                .Where(r => r.PasajeroId == pasajeroId)
                .ToListAsync();
        }

        // Detalle del viaje
        public async Task<Viaje?> ObtenerDetalleAsync(int viajeId)
        {
            return await _context.Viajes
                .Include(v => v.Ruta)
                .Include(v => v.Unidad)
                .Include(v => v.Chofer)
                .Include(v => v.Reservas)
                .FirstOrDefaultAsync(v => v.ViajeId == viajeId);
        }
    }
}