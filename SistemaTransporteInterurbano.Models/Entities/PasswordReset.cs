using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaTransporteInterurbano.Models.Entities;

public class PasswordReset
{
    [Key]
    public int PasswordResetId { get; set; }

    [Required]
    public int UsuarioId { get; set; }

    [Required(ErrorMessage = "El código de recuperación es obligatorio.")]
    [StringLength(10)]
    public string Codigo { get; set; } = string.Empty;

    [Required]
    public DateTime Expiracion { get; set; }

    [ForeignKey(nameof(UsuarioId))]
    public Usuario? Usuario { get; set; }
}