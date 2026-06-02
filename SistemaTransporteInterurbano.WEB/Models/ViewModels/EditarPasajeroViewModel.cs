using System.ComponentModel.DataAnnotations;

namespace SistemaTransporteInterurbano.WEB.Models.ViewModels;

public class EditarPasajeroViewModel
{
    public int PasajeroId { get; set; }

    [Required(ErrorMessage = "La identificación es requerida.")]
    [StringLength(9, ErrorMessage = "La identificación no puede tener más de 9 dígitos.")]
    [RegularExpression(@"^\d{1,9}$", ErrorMessage = "La identificación debe contener solo números.")]
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
