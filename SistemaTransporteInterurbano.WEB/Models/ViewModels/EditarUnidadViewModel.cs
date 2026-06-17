using System.ComponentModel.DataAnnotations;

namespace SistemaTransporteInterurbano.WEB.Models.ViewModels;

public class EditarUnidadViewModel
{
    public int UnidadId { get; set; }

    [Required(ErrorMessage = "La placa es requerida.")]
    [Display(Name = "Placa")]
    public string Placa { get; set; } = string.Empty;

    [Required(ErrorMessage = "El modelo es requerido.")]
    [Display(Name = "Modelo")]
    public string Modelo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El año de fabricación es requerido.")]
    [Range(0, 2100, ErrorMessage = "Ingrese un año válido (0-2100).")]
    [Display(Name = "Año de fabricación")]
    public int AnioFabricacion { get; set; }

    [Required(ErrorMessage = "La capacidad es requerida.")]
    [Range(1, 500, ErrorMessage = "La capacidad debe ser mayor a 0.")]
    [Display(Name = "Capacidad de pasajeros")]
    public int CapacidadPasajeros { get; set; }
}
