namespace SistemaTransporteInterurbano.BL.Interfaces;

public interface INotificacionCorreoService
{
    Task EnviarCorreoAsync(
        string destino,
        string asunto,
        string mensaje);
}