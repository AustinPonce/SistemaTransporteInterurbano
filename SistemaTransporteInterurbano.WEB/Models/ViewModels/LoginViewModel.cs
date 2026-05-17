using System.ComponentModel.DataAnnotations;

namespace SistemaTransporteInterurbano.WEB.Models.ViewModels;

public class LoginViewModel
{
    [Required(
        ErrorMessage =
            "El nombre de usuario es requerido.")]
    public string NombreUsuario { get; set; }
        = string.Empty;

    [Required(
        ErrorMessage =
            "La clave es requerida.")]
    [DataType(DataType.Password)]
    public string Clave { get; set; }
        = string.Empty;
}