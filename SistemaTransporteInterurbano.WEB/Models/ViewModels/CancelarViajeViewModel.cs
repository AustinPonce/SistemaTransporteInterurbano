using System.ComponentModel.DataAnnotations;

namespace SistemaTransporteInterurbano.WEB.Models.ViewModels
{
    public class CancelarViajeViewModel
    {
        public int ViajeId { get; set; }

        public string RutaNombre { get; set; } = string.Empty;
        public string PlacaUnidad { get; set; } = string.Empty;
        public DateTime FechaSalida { get; set; }

        [Required(ErrorMessage = "El motivo de cancelación es requerido.")]
        [StringLength(500, ErrorMessage = "El motivo no puede exceder 500 caracteres.")]
        [Display(Name = "Motivo de cancelación")]
        public string Motivo { get; set; } = string.Empty;
    }
}
