using System.ComponentModel.DataAnnotations;

namespace SistemaTransporteInterurbano.UI.Models.ViewModels;

public class CambiarClaveViewModel
{
    [Required(
        ErrorMessage =
            "El nombre de usuario es requerido.")]
    public string NombreUsuario { get; set; }
        = string.Empty;

    [Required(
        ErrorMessage =
            "La clave actual es requerida.")]
    [DataType(DataType.Password)]
    public string ClaveActual { get; set; }
        = string.Empty;

    [Required(
        ErrorMessage =
            "La nueva clave es requerida.")]
    [DataType(DataType.Password)]
    public string NuevaClave { get; set; }
        = string.Empty;
}