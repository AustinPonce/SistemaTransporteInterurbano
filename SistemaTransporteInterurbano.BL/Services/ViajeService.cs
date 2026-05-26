// BL/Services/ViajeService.cs
using Microsoft.EntityFrameworkCore;
using SistemaTransporteInterurbano.BL.Interfaces;
using SistemaTransporteInterurbano.DA.Context;
using SistemaTransporteInterurbano.Models.Entities;

namespace SistemaTransporteInterurbano.BL.Services
{
    public class ViajeService : IViajeService
    {
        private readonly AppDbContext _context;
        private readonly INotificacionCorreoService _correoService;

        public ViajeService(AppDbContext context, INotificacionCorreoService correoService)
        {
            _context = context;
            _correoService = correoService;
        }

        // ══════════════════════════════════════════════════════
        // MÓDULO 6 — Gestión de Viajes
        // ══════════════════════════════════════════════════════

        // REQ #24 — Listar viajes con filtro por ruta o fecha
        public async Task<List<Viaje>> ListarViajesAsync(
            string? filtroRuta = null,
            DateTime? filtroFecha = null)
        {
            var query = _context.Viajes
                .Include(v => v.Ruta)
                .Include(v => v.Unidad)
                .Include(v => v.Chofer)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtroRuta))
                query = query.Where(v => v.Ruta.Nombre.Contains(filtroRuta));

            if (filtroFecha.HasValue)
                query = query.Where(v => v.FechaSalida.Date == filtroFecha.Value.Date);

            return await query
                .OrderBy(v => v.FechaSalida)
                .ToListAsync();
        }

        // REQ #25-27 — Agregar viaje
        public async Task<(bool exito, string mensaje)> AgregarViajeAsync(Viaje viaje)
        {
            // REQ #26 — Validar traslape del CHOFER
            bool choferOcupado = await TieneViajeActivoEnRangoAsync(
                choferId: viaje.ChoferId,
                unidadId: null,
                salida: viaje.FechaSalida,
                llegada: viaje.FechaLlegadaEstimada,
                excluirViajeId: null);

            if (choferOcupado)
                return (false, "El chofer ya tiene un viaje activo en ese rango de fechas y horas.");

            // REQ #26 — Validar traslape de la UNIDAD
            bool unidadOcupada = await TieneViajeActivoEnRangoAsync(
                choferId: null,
                unidadId: viaje.UnidadId,
                salida: viaje.FechaSalida,
                llegada: viaje.FechaLlegadaEstimada,
                excluirViajeId: null);

            if (unidadOcupada)
                return (false, "La unidad ya está asignada a otro viaje activo en ese rango de fechas y horas.");

            // REQ #27 — Estado inicial y número correlativo
            viaje.Estado = EstadoViaje.Programado;
            viaje.NumeroViaje = await GenerarNumeroViajeAsync();

            _context.Viajes.Add(viaje);
            await _context.SaveChangesAsync();
            return (true, "Viaje registrado correctamente.");
        }

        // REQ #28-29 — Editar viaje (solo si está Programado)
        public async Task<(bool exito, string mensaje)> EditarViajeAsync(Viaje viaje)
        {
            var existente = await _context.Viajes.FindAsync(viaje.ViajeId);

            if (existente is null)
                return (false, "El viaje no existe.");

            // REQ #29 — Restricción de estado
            if (existente.Estado != EstadoViaje.Programado)
                return (false, "Solo se pueden editar viajes en estado Programado.");

            // REQ #26 reutilizado — Revalidar traslapes excluyendo el viaje actual
            bool choferOcupado = await TieneViajeActivoEnRangoAsync(
                choferId: viaje.ChoferId,
                unidadId: null,
                salida: viaje.FechaSalida,
                llegada: viaje.FechaLlegadaEstimada,
                excluirViajeId: viaje.ViajeId);

            if (choferOcupado)
                return (false, "El chofer ya tiene un viaje activo en ese rango de fechas y horas.");

            bool unidadOcupada = await TieneViajeActivoEnRangoAsync(
                choferId: null,
                unidadId: viaje.UnidadId,
                salida: viaje.FechaSalida,
                llegada: viaje.FechaLlegadaEstimada,
                excluirViajeId: viaje.ViajeId);

            if (unidadOcupada)
                return (false, "La unidad ya está asignada a otro viaje activo en ese rango de fechas y horas.");

            // Aplicar cambios solo a campos editables
            existente.RutaId = viaje.RutaId;
            existente.UnidadId = viaje.UnidadId;
            existente.ChoferId = viaje.ChoferId;
            existente.FechaSalida = viaje.FechaSalida;
            existente.FechaLlegadaEstimada = viaje.FechaLlegadaEstimada;

            await _context.SaveChangesAsync();
            return (true, "Viaje actualizado correctamente.");
        }

        // REQ #30-31 — Cancelar viaje con motivo y notificación a pasajeros
        public async Task<(bool exito, string mensaje)> CancelarViajeAsync(
            int viajeId,
            string motivo)
        {
            // REQ #30 — Motivo obligatorio
            if (string.IsNullOrWhiteSpace(motivo))
                return (false, "Debe ingresar un motivo de cancelación.");

            var viaje = await _context.Viajes
                .Include(v => v.Ruta)
                .Include(v => v.Reservas)
                    .ThenInclude(r => r.Pasajero) // Asume Pasajero tiene Nombre y Correo
                .FirstOrDefaultAsync(v => v.ViajeId == viajeId);

            if (viaje is null)
                return (false, "El viaje no existe.");

            // REQ #30 — Solo viajes Programados
            if (viaje.Estado != EstadoViaje.Programado)
                return (false, "Solo se pueden cancelar viajes en estado Programado.");

            viaje.Estado = EstadoViaje.Cancelado;
            viaje.MotivoCancelacion = motivo;
            await _context.SaveChangesAsync();

            // REQ #31 — Notificar a cada pasajero con reserva activa
            foreach (var reserva in viaje.Reservas)
            {
                await _correoService.EnviarCorreoAsync(
                    destino: reserva.Pasajero.CorreoElectronico,
                    asunto: $"Viaje cancelado — {viaje.Ruta.Nombre}",
                    mensaje: $"Estimado/a {reserva.Pasajero.Nombre},\n\n" +
                            $"Le informamos que el viaje #{viaje.NumeroViaje} con ruta " +
                            $"{viaje.Ruta.Nombre}, programado para el " +
                            $"{viaje.FechaSalida:dd/MM/yyyy} a las {viaje.FechaSalida:HH:mm}, " +
                            $"ha sido cancelado.\n\n" +
                            $"Motivo: {motivo}"
                );
            }

            return (true, "Viaje cancelado y pasajeros notificados.");
        }

        // REQ #32 — Iniciar viaje (Programado → En Curso)
        public async Task<(bool exito, string mensaje)> IniciarViajeAsync(int viajeId)
        {
            var viaje = await _context.Viajes.FindAsync(viajeId);

            if (viaje is null)
                return (false, "El viaje no existe.");

            if (viaje.Estado != EstadoViaje.Programado)
                return (false, "Solo se pueden iniciar viajes en estado Programado.");

            viaje.Estado = EstadoViaje.EnCurso;
            await _context.SaveChangesAsync();
            return (true, "El viaje ha iniciado correctamente.");
        }

        // Obtener viaje por Id (usado en vistas de edición/cancelación)
        public async Task<Viaje?> ObtenerPorIdAsync(int viajeId)
        {
            return await _context.Viajes
                .Include(v => v.Ruta)
                .Include(v => v.Unidad)
                .Include(v => v.Chofer)
                .FirstOrDefaultAsync(v => v.ViajeId == viajeId);
        }

        // ══════════════════════════════════════════════════════
        // MÓDULO 7 — Viajes en Curso (tu código original)
        // ══════════════════════════════════════════════════════

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
                .FirstOrDefaultAsync(v => v.ViajeId == viajeId);

            if (viaje == null)
                throw new Exception("Viaje no encontrado.");

            if (viaje.Estado != EstadoViaje.EnCurso)
                throw new Exception("El viaje no está en curso.");

            bool asientoOcupado = await _context.Reservas
                .AnyAsync(r => r.ViajeId == viajeId && r.NumeroAsiento == asiento);

            if (asientoOcupado)
                throw new Exception("Ese asiento ya está ocupado.");

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
            var reserva = await _context.Reservas
                .FirstOrDefaultAsync(r => r.ReservaId == reservaId);

            if (reserva == null)
                throw new Exception("Reserva no encontrada.");

            _context.Reservas.Remove(reserva);
            await _context.SaveChangesAsync();
        }

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

        public async Task<(int pasajeros, int disponibles, decimal total)> ObtenerTotalesAsync(int viajeId)
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

        // ══════════════════════════════════════════════════════
        // MÓDULO 9 — Mis Viajes (tu código original)
        // ══════════════════════════════════════════════════════

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

        // ══════════════════════════════════════════════════════
        // Métodos auxiliares privados
        // ══════════════════════════════════════════════════════

        // Detecta traslape de horario para chofer o unidad (REQ #26)
        private async Task<bool> TieneViajeActivoEnRangoAsync(
            int? choferId,
            int? unidadId,
            DateTime salida,
            DateTime llegada,
            int? excluirViajeId)
        {
            var query = _context.Viajes
                .Where(v =>
                    (v.Estado == EstadoViaje.Programado || v.Estado == EstadoViaje.EnCurso) &&
                    salida < v.FechaLlegadaEstimada &&   // lógica estándar de traslape
                    llegada > v.FechaSalida);

            if (excluirViajeId.HasValue)
                query = query.Where(v => v.ViajeId != excluirViajeId.Value);

            if (choferId.HasValue)
                query = query.Where(v => v.ChoferId == choferId.Value);

            if (unidadId.HasValue)
                query = query.Where(v => v.UnidadId == unidadId.Value);

            return await query.AnyAsync();
        }

        // Genera el siguiente número correlativo de viaje
        private async Task<int> GenerarNumeroViajeAsync()
        {
            var ultimo = await _context.Viajes
                .OrderByDescending(v => v.NumeroViaje)
                .Select(v => v.NumeroViaje)
                .FirstOrDefaultAsync();

            return ultimo + 1;
        }
    }
}