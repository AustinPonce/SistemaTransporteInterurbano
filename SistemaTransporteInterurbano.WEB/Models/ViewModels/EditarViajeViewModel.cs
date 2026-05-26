using System.ComponentModel.DataAnnotations;

namespace SistemaTransporteInterurbano.WEB.Models.ViewModels
{
    public class EditarViajeViewModel
    {
        public int ViajeId { get; set; }

        [Required(ErrorMessage = "La ruta es requerida.")]
        [Display(Name = "Ruta")]
        public int RutaId { get; set; }

        [Required(ErrorMessage = "La unidad es requerida.")]
        [Display(Name = "Unidad")]
        public int UnidadId { get; set; }

        [Required(ErrorMessage = "El chofer es requerido.")]
        [Display(Name = "Chofer asignado")]
        public int ChoferId { get; set; }

        [Required(ErrorMessage = "La fecha y hora de salida es requerida.")]
        [Display(Name = "Fecha y hora de salida")]
        public DateTime FechaSalida { get; set; }

        [Required(ErrorMessage = "La fecha y hora estimada de llegada es requerida.")]
        [Display(Name = "Fecha y hora estimada de llegada")]
        public DateTime FechaLlegadaEstimada { get; set; }
    }
}
