namespace SistemaTransporteInterurbano.BL.Interfaces;

public interface IUsuarioService
{
    Task RegistrarUsuarioChofer(
        string nombreUsuario,
        string correoElectronico,
        int rolId);
}