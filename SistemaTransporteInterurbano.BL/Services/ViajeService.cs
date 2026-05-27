using Microsoft.EntityFrameworkCore;
using SistemaTransporteInterurbano.BL.Interfaces;
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

        public async Task<List<Viaje>> ObtenerTodosAsync(string? filtroRuta, DateTime? filtroFecha)
        {
            var query = _context.Viajes
                .Include(v => v.Ruta)
                .Include(v => v.Unidad)
                .Include(v => v.Chofer)
                .Include(v => v.Reservas)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtroRuta))
                query = query.Where(v => v.Ruta.Nombre.Contains(filtroRuta));

            if (filtroFecha.HasValue)
                query = query.Where(v => v.FechaSalida.Date == filtroFecha.Value.Date);

            return await query.OrderBy(v => v.FechaSalida).ToListAsync();
        }

        public async Task<Viaje?> ObtenerPorIdAsync(int id)
        {
            return await _context.Viajes
                .Include(v => v.Ruta)
                .Include(v => v.Unidad)
                .Include(v => v.Chofer)
                .Include(v => v.Reservas)
                    .ThenInclude(r => r.Pasajero)
                .FirstOrDefaultAsync(v => v.ViajeId == id);
        }

        public async Task AgregarAsync(int rutaId, int unidadId, int choferId,
                                       DateTime fechaSalida, DateTime fechaLlegada)
        {
            if (fechaLlegada <= fechaSalida)
                throw new Exception("La fecha de llegada debe ser posterior a la de salida.");

            bool unidadOcupada = await _context.Viajes.AnyAsync(v =>
                v.UnidadId == unidadId &&
                (v.Estado == EstadoViaje.Programado || v.Estado == EstadoViaje.EnCurso) &&
                v.FechaSalida < fechaLlegada && v.FechaLlegadaEstimada > fechaSalida);

            if (unidadOcupada)
                throw new Exception("La unidad seleccionada ya tiene un viaje activo en ese rango de fechas.");

            bool choferOcupado = await _context.Viajes.AnyAsync(v =>
                v.ChoferId == choferId &&
                (v.Estado == EstadoViaje.Programado || v.Estado == EstadoViaje.EnCurso) &&
                v.FechaSalida < fechaLlegada && v.FechaLlegadaEstimada > fechaSalida);

            if (choferOcupado)
                throw new Exception("El chofer seleccionado ya tiene un viaje activo en ese rango de fechas.");

            var viaje = new Viaje
            {
                RutaId = rutaId,
                UnidadId = unidadId,
                ChoferId = choferId,
                FechaSalida = fechaSalida,
                FechaLlegadaEstimada = fechaLlegada,
                Estado = EstadoViaje.Programado
            };

            _context.Viajes.Add(viaje);
            await _context.SaveChangesAsync();
        }

        public async Task EditarAsync(int id, int rutaId, int unidadId, int choferId,
                                      DateTime fechaSalida, DateTime fechaLlegada)
        {
            var viaje = await _context.Viajes.FindAsync(id)
                ?? throw new Exception("Viaje no encontrado.");

            if (viaje.Estado != EstadoViaje.Programado)
                throw new Exception("Solo se pueden editar viajes en estado Programado.");

            if (fechaLlegada <= fechaSalida)
                throw new Exception("La fecha de llegada debe ser posterior a la de salida.");

            bool unidadOcupada = await _context.Viajes.AnyAsync(v =>
                v.ViajeId != id &&
                v.UnidadId == unidadId &&
                (v.Estado == EstadoViaje.Programado || v.Estado == EstadoViaje.EnCurso) &&
                v.FechaSalida < fechaLlegada && v.FechaLlegadaEstimada > fechaSalida);

            if (unidadOcupada)
                throw new Exception("La unidad ya tiene un viaje activo en ese rango de fechas.");

            bool choferOcupado = await _context.Viajes.AnyAsync(v =>
                v.ViajeId != id &&
                v.ChoferId == choferId &&
                (v.Estado == EstadoViaje.Programado || v.Estado == EstadoViaje.EnCurso) &&
                v.FechaSalida < fechaLlegada && v.FechaLlegadaEstimada > fechaSalida);

            if (choferOcupado)
                throw new Exception("El chofer ya tiene un viaje activo en ese rango de fechas.");

            viaje.RutaId = rutaId;
            viaje.UnidadId = unidadId;
            viaje.ChoferId = choferId;
            viaje.FechaSalida = fechaSalida;
            viaje.FechaLlegadaEstimada = fechaLlegada;

            await _context.SaveChangesAsync();
        }

        public async Task CancelarAsync(int id, string motivo, INotificacionCorreoService correoService)
        {
            var viaje = await _context.Viajes
                .Include(v => v.Reservas)
                    .ThenInclude(r => r.Pasajero)
                .Include(v => v.Ruta)
                .FirstOrDefaultAsync(v => v.ViajeId == id)
                ?? throw new Exception("Viaje no encontrado.");

            if (viaje.Estado != EstadoViaje.Programado)
                throw new Exception("Solo se pueden cancelar viajes en estado Programado.");

            viaje.Estado = EstadoViaje.Cancelado;
            viaje.MotivoCancelacion = motivo;
            await _context.SaveChangesAsync();

            foreach (var reserva in viaje.Reservas)
            {
                if (reserva.Pasajero != null)
                {
                    try
                    {
                        await correoService.EnviarCorreoAsync(
                            reserva.Pasajero.CorreoElectronico,
                            $"Viaje cancelado — {viaje.Ruta?.Nombre}",
                            $"Estimado/a {reserva.Pasajero.Nombre} {reserva.Pasajero.Apellidos},\n\n" +
                            $"Le informamos que el viaje de la ruta \"{viaje.Ruta?.Nombre}\" " +
                            $"programado para el {viaje.FechaSalida:dd/MM/yyyy} a las {viaje.FechaSalida:HH:mm} " +
                            $"ha sido cancelado.\n\nMotivo: {motivo}\n\nDisculpe los inconvenientes."
                        );
                    }
                    catch { }
                }
            }
        }

        public async Task IniciarAsync(int id)
        {
            var viaje = await _context.Viajes.FindAsync(id)
                ?? throw new Exception("Viaje no encontrado.");

            if (viaje.Estado != EstadoViaje.Programado)
                throw new Exception("Solo se pueden iniciar viajes en estado Programado.");

            viaje.Estado = EstadoViaje.EnCurso;
            await _context.SaveChangesAsync();
        }

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

        public async Task ReservarAsientoAsync(int viajeId, int pasajeroId, int asiento)
        {
            var viaje = await _context.Viajes
                .Include(v => v.Ruta)
                .Include(v => v.Unidad)
                .Include(v => v.Reservas)
                .FirstOrDefaultAsync(v => v.ViajeId == viajeId)
                ?? throw new Exception("Viaje no encontrado.");

            if (viaje.Estado != EstadoViaje.EnCurso)
                throw new Exception("El viaje no está en curso.");

            bool asientoOcupado = viaje.Reservas.Any(r => r.NumeroAsiento == asiento);
            if (asientoOcupado)
                throw new Exception("Ese asiento ya está ocupado.");

            if (viaje.Reservas.Count >= viaje.Unidad.CapacidadPasajeros)
                throw new Exception("Capacidad máxima alcanzada.");

            _context.Reservas.Add(new Reserva
            {
                ViajeId = viajeId,
                PasajeroId = pasajeroId,
                NumeroAsiento = asiento,
                MontoPagado = viaje.Ruta.PrecioBase
            });

            await _context.SaveChangesAsync();
        }

        public async Task<List<Reserva>> ObtenerPasajerosAsync(int viajeId)
        {
            return await _context.Reservas
                .Include(r => r.Pasajero)
                .Where(r => r.ViajeId == viajeId)
                .OrderBy(r => r.NumeroAsiento)
                .ToListAsync();
        }

        public async Task CancelarReservaAsync(int reservaId)
        {
            var reserva = await _context.Reservas.FindAsync(reservaId)
                ?? throw new Exception("Reserva no encontrada.");

            _context.Reservas.Remove(reserva);
            await _context.SaveChangesAsync();
        }

        public async Task FinalizarViajeAsync(int viajeId)
        {
            var viaje = await _context.Viajes.FindAsync(viajeId)
                ?? throw new Exception("Viaje no encontrado.");

            if (viaje.Estado != EstadoViaje.EnCurso)
                throw new Exception("Solo se pueden finalizar viajes en curso.");

            viaje.Estado = EstadoViaje.Completado;
            await _context.SaveChangesAsync();
        }

        public async Task<(int pasajeros, int disponibles, decimal total)> ObtenerTotalesAsync(int viajeId)
        {
            var viaje = await _context.Viajes
                .Include(v => v.Unidad)
                .Include(v => v.Reservas)
                .FirstOrDefaultAsync(v => v.ViajeId == viajeId)
                ?? throw new Exception("Viaje no encontrado.");

            int pasajeros = viaje.Reservas.Count;
            int disponibles = viaje.Unidad.CapacidadPasajeros - pasajeros;
            decimal total = viaje.Reservas.Sum(r => r.MontoPagado);
            return (pasajeros, disponibles, total);
        }

        public async Task<List<Viaje>> ObtenerCanceladosAsync()
        {
            return await _context.Viajes
                .Include(v => v.Ruta)
                .Include(v => v.Unidad)
                .Include(v => v.Chofer)
                .Where(v => v.Estado == EstadoViaje.Cancelado)
                .OrderByDescending(v => v.FechaSalida)
                .ToListAsync();
        }

        public async Task<List<Reserva>> ObtenerReservasPasajeroAsync(int pasajeroId)
        {
            return await _context.Reservas
                .Include(r => r.Viaje)
                    .ThenInclude(v => v.Ruta)
                .Where(r => r.PasajeroId == pasajeroId)
                .ToListAsync();
        }

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
