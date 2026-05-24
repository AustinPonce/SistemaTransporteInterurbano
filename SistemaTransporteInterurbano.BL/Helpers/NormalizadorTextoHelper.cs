namespace SistemaTransporteInterurbano.BL.Helpers;

public static class NormalizadorTextoHelper
{
    public static string Normalizar(string texto)
    {
        var normalizado = texto
            .ToLower()
            .Normalize(System.Text.NormalizationForm.FormD);

        return new string(normalizado
            .Where(c => System.Globalization.CharUnicodeInfo
                .GetUnicodeCategory(c) !=
                System.Globalization.UnicodeCategory.NonSpacingMark)
            .ToArray());
    }

    public static bool Contiene(string texto, string filtro)
    {
        return Normalizar(texto).Contains(Normalizar(filtro));
    }
}