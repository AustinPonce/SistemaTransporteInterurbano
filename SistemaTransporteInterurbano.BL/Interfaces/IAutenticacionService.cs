using SistemaTransporteInterurbano.Models.Entities;

namespace SistemaTransporteInterurbano.BL.Interfaces;

public interface IAutenticacionService
{
    Task<Usuario?> AutenticarUsuarioPorNombreYClave(
        string nombreUsuario,
        string clave);

    Task CambiarClaveDeUsuario(
        string nombreUsuario,
        string claveActual,
        string nuevaClave);
}