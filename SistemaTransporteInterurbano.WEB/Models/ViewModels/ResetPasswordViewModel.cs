using System.ComponentModel.DataAnnotations;

namespace SistemaTransporteInterurbano.WEB.Models.ViewModels;

public class ResetPasswordViewModel
{
    [Required(ErrorMessage = "El correo es requerido.")]
    [EmailAddress(ErrorMessage = "Ingrese un correo válido.")]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El código es requerido.")]
    public string Codigo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La nueva clave es requerida.")]
    [DataType(DataType.Password)]
    public string NuevaClave { get; set; } = string.Empty;

    [Required(ErrorMessage = "La confirmación es requerida.")]
    [DataType(DataType.Password)]
    [Compare("NuevaClave", ErrorMessage = "Las claves no coinciden.")]
    public string ConfirmarClave { get; set; } = string.Empty;
}
