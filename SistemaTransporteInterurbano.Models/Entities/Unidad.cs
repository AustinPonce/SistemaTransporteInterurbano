using System.ComponentModel.DataAnnotations;

namespace SistemaTransporteInterurbano.Models.Entities;

public class Unidad
{
    [Key]
    public int UnidadId { get; set; }

    [Required(ErrorMessage = "La placa es requerida.")]
    [StringLength(20)]
    public string Placa { get; set; } = string.Empty;

    [Required(ErrorMessage = "El modelo es requerido.")]
    [StringLength(100)]
    public string Modelo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El año de fabricación es requerido.")]
    [Range(1900, 2100, ErrorMessage = "Ingrese un año válido.")]
    public int AnioFabricacion { get; set; }

    [Required(ErrorMessage = "La capacidad es requerida.")]
    [Range(1, 500, ErrorMessage = "La capacidad debe ser mayor a 0.")]
    public int CapacidadPasajeros { get; set; }
}