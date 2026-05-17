namespace SistemaTransporteInterurbano.BL.Helpers;

public static class GeneradorClaveHelper
{
    public static string GenerarClaveTemporal()
    {
        return Guid
            .NewGuid()
            .ToString("N")
            .Substring(0, 8);
    }
}