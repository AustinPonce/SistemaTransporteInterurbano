using System.ComponentModel.DataAnnotations;

namespace SistemaTransporteInterurbano.WEB.Models.ViewModels;

public class AgregarUnidadViewModel
{
    [Required(ErrorMessage = "La placa es requerida.")]
    [Display(Name = "Placa")]
    public string Placa { get; set; } = string.Empty;

    [Required(ErrorMessage = "El modelo es requerido.")]
    [Display(Name = "Modelo")]
    public string Modelo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El año de fabricación es requerido.")]
    [Range(1900, 2100, ErrorMessage = "Ingrese un año válido.")]
    [Display(Name = "Año de fabricación")]
    public int AnioFabricacion { get; set; }

    [Required(ErrorMessage = "La capacidad es requerida.")]
    [Range(1, 500, ErrorMessage = "La capacidad debe ser mayor a 0.")]
    [Display(Name = "Capacidad de pasajeros")]
    public int CapacidadPasajeros { get; set; }
}