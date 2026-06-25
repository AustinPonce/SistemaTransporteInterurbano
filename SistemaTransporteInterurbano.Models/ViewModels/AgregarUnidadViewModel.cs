using System.ComponentModel.DataAnnotations;

namespace SistemaTransporteInterurbano.Models.ViewModels;

public class AgregarUnidadViewModel
{
    [Required(ErrorMessage = "La placa es requerida.")]
    [Display(Name = "Placa")]
    public string Placa { get; set; } = string.Empty;

    [Required(ErrorMessage = "El modelo es requerido.")]
    [Display(Name = "Modelo")]
    public string Modelo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El año de fabricación es requerido.")]
    [Range(1980, 2027, ErrorMessage = "Ingrese un año entre 1980 y 2027.")]
    [Display(Name = "Año de fabricación")]
    public int AnioFabricacion { get; set; }

    [Required(ErrorMessage = "La capacidad es requerida.")]
    [Range(8, 80, ErrorMessage = "La capacidad debe estar entre 8 y 80 pasajeros.")]
    [Display(Name = "Capacidad de pasajeros")]
    public int CapacidadPasajeros { get; set; }
}