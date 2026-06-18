namespace SistemaTransporteInterurbano.WEB.Models;

public class ApiRespuesta<T>
{
    public bool Exitoso { get; set; }
    public T? Datos { get; set; }
    public string? Mensaje { get; set; }

    public static ApiRespuesta<T> Exito(T datos, string? mensaje = null) =>
        new() { Exitoso = true, Datos = datos, Mensaje = mensaje };

    public static ApiRespuesta<T> Error(string mensaje) =>
        new() { Exitoso = false, Datos = default, Mensaje = mensaje };
}
