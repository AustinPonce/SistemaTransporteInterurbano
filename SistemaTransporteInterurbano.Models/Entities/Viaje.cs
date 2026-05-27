using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaTransporteInterurbano.Models.Entities
{
    public class Viaje
    {
        public int ViajeId { get; set; }

        public int RutaId { get; set; }
        public Ruta Ruta { get; set; } = null!;

        public int UnidadId { get; set; }
        public Unidad Unidad { get; set; } = null!;

        public int ChoferId { get; set; }
        public Chofer Chofer { get; set; } = null!;

        public DateTime FechaSalida { get; set; }
        public DateTime FechaLlegadaEstimada { get; set; }

        public EstadoViaje Estado { get; set; }

        public string? MotivoCancelacion { get; set; }

        public List<Reserva> Reservas { get; set; } = new();
    }
}
