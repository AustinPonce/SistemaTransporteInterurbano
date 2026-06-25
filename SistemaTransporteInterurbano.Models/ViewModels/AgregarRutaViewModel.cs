using System.ComponentModel.DataAnnotations;

namespace SistemaTransporteInterurbano.Models.ViewModels;

public class AgregarRutaViewModel
{
    [Required(ErrorMessage = "El nombre es requerido.")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El origen es requerido.")]
    [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El origen no puede contener números.")]
    [Display(Name = "Origen")]
    public string Origen { get; set; } = string.Empty;

    [Required(ErrorMessage = "El destino es requerido.")]
    [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El destino no puede contener números.")]
    [Display(Name = "Destino")]
    public string Destino { get; set; } = string.Empty;

    [Required(ErrorMessage = "La duración estimada es requerida.")]
    [RegularExpression(@"^\d{2}:\d{2}$", ErrorMessage = "Formato inválido. Use hh:mm.")]
    [Display(Name = "Duración estimada (hh:mm)")]
    public string DuracionEstimada { get; set; } = string.Empty;

    [Required(ErrorMessage = "El precio base es requerido.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0.")]
    [Display(Name = "Precio base (₡)")]
    public decimal PrecioBase { get; set; }
}
