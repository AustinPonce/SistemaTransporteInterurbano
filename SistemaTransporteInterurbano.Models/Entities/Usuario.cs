using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaTransporteInterurbano.Models.Entities;

public class Usuario
{
    [Key]
    public int UsuarioId { get; set; }

    [Required(
        ErrorMessage =
            "El nombre de usuario es requerido.")]
    [StringLength(
        100,
        ErrorMessage =
            "El nombre de usuario no puede superar 100 caracteres.")]
    public string NombreUsuario { get; set; }
        = string.Empty;

    [Required(
        ErrorMessage =
            "La clave es requerida.")]
    [StringLength(
        255,
        ErrorMessage =
            "La clave no puede superar 255 caracteres.")]
    public string Clave { get; set; }
        = string.Empty;

    [Required(
        ErrorMessage =
            "El correo electrónico es requerido.")]
    [EmailAddress(
        ErrorMessage =
            "Debe ingresar un correo válido.")]
    [StringLength(
        150,
        ErrorMessage =
            "El correo no puede superar 150 caracteres.")]
    public string CorreoElectronico { get; set; }
        = string.Empty;

    [Required(
        ErrorMessage =
            "El rol es requerido.")]
    public int RolId { get; set; }

    [Required]
    public int IntentosFallidos { get; set; } = 0;

    [Required]
    public bool EstaBloqueado { get; set; } = false;

    public DateTime? FechaBloqueo { get; set; }

    [Required]
    public DateTime FechaCreacion { get; set; }
        = DateTime.Now;

    [Required]
    public bool DebeCambiarClave { get; set; }
        = true;

    [ForeignKey(nameof(RolId))]
    public Rol? Rol { get; set; }
}
