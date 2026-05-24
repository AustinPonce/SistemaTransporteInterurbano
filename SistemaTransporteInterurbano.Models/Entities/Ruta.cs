using System.ComponentModel.DataAnnotations;

namespace SistemaTransporteInterurbano.Models.Entities;

public class Ruta
{
    [Key]
    public int RutaId { get; set; }

    [Required(ErrorMessage = "El nombre es requerido.")]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El origen es requerido.")]
    [StringLength(100)]
    public string Origen { get; set; } = string.Empty;

    [Required(ErrorMessage = "El destino es requerido.")]
    [StringLength(100)]
    public string Destino { get; set; } = string.Empty;

    [Required(ErrorMessage = "La duración estimada es requerida.")]
    public TimeSpan DuracionEstimada { get; set; }

    [Required(ErrorMessage = "El precio base es requerido.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0.")]
    public decimal PrecioBase { get; set; }
}