using Microsoft.AspNetCore.Http;

namespace SistemaTransporteInterurbano.WEB.Helpers;

public static class SesionHelper
{
    public static bool EstaAutenticado(ISession session)
        => session.GetString("Rol") != null;

    public static string? ObtenerRol(ISession session)
        => session.GetString("Rol");

    public static bool EsAdministrador(ISession session)
        => session.GetString("Rol") == "Administrador";

    public static bool EsChofer(ISession session)
        => session.GetString("Rol") == "Chofer";

    public static bool EsPasajero(ISession session)
        => session.GetString("Rol") == "Pasajero";
}