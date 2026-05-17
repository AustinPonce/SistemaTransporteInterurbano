using BCrypt.Net;

namespace SistemaTransporteInterurbano.BL.Helpers;

public static class HashHelper
{
    public static string GenerarHash(
        string clave)
    {
        return BCrypt.Net.BCrypt
            .HashPassword(clave);
    }
}

