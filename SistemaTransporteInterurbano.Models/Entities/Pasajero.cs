using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaTransporteInterurbano.Models.Entities;

public class Pasajero
{
    [Key]
    public int PasajeroId { get; set; }

    [Required(ErrorMessage = "La identificación es requerida.")]
    [StringLength(50)]
    public string Identificacion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es requerido.")]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los apellidos son requeridos.")]
    [StringLength(150)]
    public string Apellidos { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es requerido.")]
    [EmailAddress(ErrorMessage = "Ingrese un correo válido.")]
    [StringLength(150)]
    public string CorreoElectronico { get; set; } = string.Empty;

    [Required]
    public int UsuarioId { get; set; }

    [ForeignKey(nameof(UsuarioId))]
    public Usuario? Usuario { get; set; }
}