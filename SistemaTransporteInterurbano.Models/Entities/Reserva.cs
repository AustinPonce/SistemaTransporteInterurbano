using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaTransporteInterurbano.Models.Entities
{
    public class Reserva
    {
        public int ReservaId { get; set; }

        public int ViajeId { get; set; }
        public Viaje Viaje { get; set; } = null!;

        public int PasajeroId { get; set; }
        public Pasajero Pasajero { get; set; } = null!;

        public int NumeroAsiento { get; set; }

        public decimal MontoPagado { get; set; }
    }
}
