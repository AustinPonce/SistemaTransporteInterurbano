using System.ComponentModel.DataAnnotations;

namespace SistemaTransporteInterurbano.WEB.Models.ViewModels;

public class EditarChoferViewModel
{
    public int ChoferId { get; set; }

    [Required(ErrorMessage = "La identificación es requerida.")]
    [StringLength(50, MinimumLength = 9, ErrorMessage = "La identificación debe tener al menos 9 dígitos.")]
    [RegularExpression(@"^\d{9,}$", ErrorMessage = "La identificación debe contener solo números y al menos 9 dígitos.")]
    [Display(Name = "Identificación")]
    public string Identificacion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es requerido.")]
    [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre no puede contener números.")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los apellidos son requeridos.")]
    [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "Los apellidos no pueden contener números.")]
    [Display(Name = "Apellidos")]
    public string Apellidos { get; set; } = string.Empty;
}
