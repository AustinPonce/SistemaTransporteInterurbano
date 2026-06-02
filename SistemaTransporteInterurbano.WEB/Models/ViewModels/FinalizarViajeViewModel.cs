namespace SistemaTransporteInterurbano.WEB.Models.ViewModels;

public class FinalizarViajeViewModel
{
    public int ViajeId { get; set; }
    public string RutaNombre { get; set; } = string.Empty;
    public string PlacaUnidad { get; set; } = string.Empty;
    public DateTime FechaSalida { get; set; }

    public int Pasajeros { get; set; }
    public int Disponibles { get; set; }
    public decimal TotalRecaudacion { get; set; }
}
