using System.ComponentModel.DataAnnotations;

namespace SistemaTransporteInterurbano.WEB.Models.ViewModels;

public class EditarChoferViewModel
{
    public int ChoferId { get; set; }

    [Required(ErrorMessage = "La identificación es requerida.")]
    [Display(Name = "Identificación")]
    public string Identificacion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es requerido.")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los apellidos son requeridos.")]
    [Display(Name = "Apellidos")]
    public string Apellidos { get; set; } = string.Empty;
}