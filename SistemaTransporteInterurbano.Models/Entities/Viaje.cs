// Models/Entities/Viaje.cs
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaTransporteInterurbano.Models.Entities
{
    public class Viaje
    {
        public int ViajeId { get; set; }

        public int NumeroViaje { get; set; }

        public int RutaId { get; set; }
        public Ruta Ruta { get; set; }

        public int UnidadId { get; set; }
        public Unidad Unidad { get; set; }

        public int ChoferId { get; set; }
        public Chofer Chofer { get; set; }

        public DateTime FechaSalida { get; set; }
        public DateTime FechaLlegadaEstimada { get; set; }

        public EstadoViaje Estado { get; set; } = EstadoViaje.Programado;

        public string? MotivoCancelacion { get; set; }

        public List<Reserva> Reservas { get; set; } = new();
    }
}