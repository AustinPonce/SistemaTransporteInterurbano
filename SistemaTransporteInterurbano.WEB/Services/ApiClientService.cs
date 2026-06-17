using System.Net.Http.Json;
using System.Text.Json;
using SistemaTransporteInterurbano.Models.Entities;
using SistemaTransporteInterurbano.WEB.Models;

namespace SistemaTransporteInterurbano.WEB.Services;

public class ApiClientService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly string _apiKey;

    public ApiClientService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _apiKey = configuration["ApiSettings:ApiKey"]!;
    }

    private string BaseUrl =>
        $"{_httpContextAccessor.HttpContext!.Request.Scheme}://{_httpContextAccessor.HttpContext!.Request.Host}";

    private async Task<ApiResponse<T>> SendAsync<T>(HttpMethod method, string endpoint, object? body = null)
    {
        var request = new HttpRequestMessage(method, $"{BaseUrl}{endpoint}");
        request.Headers.Add("X-API-Key", _apiKey);

        if (body != null)
            request.Content = JsonContent.Create(body);

        var response = await _httpClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ApiResponse<T>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (result == null)
            throw new Exception("Error al procesar la respuesta de la API.");

        if (!result.Success)
            throw new Exception(result.Message ?? "Error en la solicitud a la API.");

        return result;
    }

    private async Task<T> SendAndGetDataAsync<T>(HttpMethod method, string endpoint, object? body = null)
    {
        var response = await SendAsync<T>(method, endpoint, body);
        return response.Data!;
    }

    private async Task SendAndVerifyAsync(HttpMethod method, string endpoint, object? body = null)
    {
        await SendAsync<object>(method, endpoint, body);
    }

    public async Task<List<Chofer>> ObtenerChoferesAsync(string? filtro = null)
    {
        var query = string.IsNullOrWhiteSpace(filtro) ? "" : $"?filtroNombre={Uri.EscapeDataString(filtro)}";
        return await SendAndGetDataAsync<List<Chofer>>(HttpMethod.Get, $"/api/choferes{query}") ?? [];
    }

    public async Task<Chofer> ObtenerChoferPorIdAsync(int id) =>
        await SendAndGetDataAsync<Chofer>(HttpMethod.Get, $"/api/choferes/{id}");

    public async Task AgregarChoferAsync(string identificacion, string nombre, string apellidos, string correo) =>
        await SendAndVerifyAsync(HttpMethod.Post, "/api/choferes", new { identificacion, nombre, apellidos, correoElectronico = correo });

    public async Task EditarChoferAsync(int id, string identificacion, string nombre, string apellidos) =>
        await SendAndVerifyAsync(HttpMethod.Put, $"/api/choferes/{id}", new { identificacion, nombre, apellidos });

    public async Task EliminarChoferAsync(int id) =>
        await SendAndVerifyAsync(HttpMethod.Delete, $"/api/choferes/{id}");

    public async Task<List<Pasajero>> ObtenerPasajerosAsync(string? filtro = null)
    {
        var query = string.IsNullOrWhiteSpace(filtro) ? "" : $"?filtroNombre={Uri.EscapeDataString(filtro)}";
        return await SendAndGetDataAsync<List<Pasajero>>(HttpMethod.Get, $"/api/pasajeros{query}") ?? [];
    }

    public async Task<Pasajero> ObtenerPasajeroPorIdAsync(int id) =>
        await SendAndGetDataAsync<Pasajero>(HttpMethod.Get, $"/api/pasajeros/{id}");

    public async Task<Pasajero> ObtenerPasajeroPorUsuarioIdAsync(int usuarioId) =>
        await SendAndGetDataAsync<Pasajero>(HttpMethod.Get, $"/api/pasajeros/por-usuario/{usuarioId}");

    public async Task AgregarPasajeroAsync(string identificacion, string nombre, string apellidos, string correo) =>
        await SendAndVerifyAsync(HttpMethod.Post, "/api/pasajeros", new { identificacion, nombre, apellidos, correoElectronico = correo });

    public async Task EditarPasajeroAsync(int id, string identificacion, string nombre, string apellidos) =>
        await SendAndVerifyAsync(HttpMethod.Put, $"/api/pasajeros/{id}", new { identificacion, nombre, apellidos });

    public async Task<List<Ruta>> ObtenerRutasAsync(string? filtro = null)
    {
        var query = string.IsNullOrWhiteSpace(filtro) ? "" : $"?filtro={Uri.EscapeDataString(filtro)}";
        return await SendAndGetDataAsync<List<Ruta>>(HttpMethod.Get, $"/api/rutas{query}") ?? [];
    }

    public async Task<Ruta> ObtenerRutaPorIdAsync(int id) =>
        await SendAndGetDataAsync<Ruta>(HttpMethod.Get, $"/api/rutas/{id}");

    public async Task AgregarRutaAsync(string nombre, string origen, string destino, TimeSpan duracion, decimal precioBase) =>
        await SendAndVerifyAsync(HttpMethod.Post, "/api/rutas", new { nombre, origen, destino, duracionEstimada = duracion.ToString(@"hh\:mm"), precioBase });

    public async Task EditarRutaAsync(int id, string nombre, string origen, string destino, TimeSpan duracion, decimal precioBase) =>
        await SendAndVerifyAsync(HttpMethod.Put, $"/api/rutas/{id}", new { nombre, origen, destino, duracionEstimada = duracion.ToString(@"hh\:mm"), precioBase });

    public async Task<List<Unidad>> ObtenerUnidadesAsync() =>
        await SendAndGetDataAsync<List<Unidad>>(HttpMethod.Get, "/api/unidades") ?? [];

    public async Task<Unidad> ObtenerUnidadPorIdAsync(int id) =>
        await SendAndGetDataAsync<Unidad>(HttpMethod.Get, $"/api/unidades/{id}");

    public async Task AgregarUnidadAsync(string placa, string modelo, int anio, int capacidad) =>
        await SendAndVerifyAsync(HttpMethod.Post, "/api/unidades", new { placa, modelo, anioFabricacion = anio, capacidadPasajeros = capacidad });

    public async Task EditarUnidadAsync(int id, string placa, string modelo, int anio, int capacidad) =>
        await SendAndVerifyAsync(HttpMethod.Put, $"/api/unidades/{id}", new { placa, modelo, anioFabricacion = anio, capacidadPasajeros = capacidad });

    public async Task<List<Viaje>> ObtenerViajesAsync(string? filtroRuta, DateTime? filtroFecha)
    {
        var query = $"?filtroRuta={Uri.EscapeDataString(filtroRuta ?? "")}&filtroFecha={filtroFecha?.ToString("yyyy-MM-dd")}";
        return await SendAndGetDataAsync<List<Viaje>>(HttpMethod.Get, $"/api/viajes{query}") ?? [];
    }

    public async Task<Viaje> ObtenerViajePorIdAsync(int id) =>
        await SendAndGetDataAsync<Viaje>(HttpMethod.Get, $"/api/viajes/{id}");

    public async Task<Viaje> ObtenerDetalleViajeAsync(int id) =>
        await SendAndGetDataAsync<Viaje>(HttpMethod.Get, $"/api/viajes/{id}/detalle");

    public async Task AgregarViajeAsync(int rutaId, int unidadId, int choferId, DateTime fechaSalida, DateTime fechaLlegada) =>
        await SendAndVerifyAsync(HttpMethod.Post, "/api/viajes", new { rutaId, unidadId, choferId, fechaSalida, fechaLlegadaEstimada = fechaLlegada });

    public async Task EditarViajeAsync(int id, int rutaId, int unidadId, int choferId, DateTime fechaSalida, DateTime fechaLlegada) =>
        await SendAndVerifyAsync(HttpMethod.Put, $"/api/viajes/{id}", new { rutaId, unidadId, choferId, fechaSalida, fechaLlegadaEstimada = fechaLlegada });

    public async Task CancelarViajeAsync(int id, string motivo) =>
        await SendAndVerifyAsync(HttpMethod.Post, $"/api/viajes/{id}/cancelar", new { motivo });

    public async Task IniciarViajeAsync(int id) =>
        await SendAndVerifyAsync(HttpMethod.Post, $"/api/viajes/{id}/iniciar");

    public async Task FinalizarViajeAsync(int id) =>
        await SendAndVerifyAsync(HttpMethod.Post, $"/api/viajes/{id}/finalizar");

    public async Task<List<Viaje>> ObtenerViajesActivosAsync() =>
        await SendAndGetDataAsync<List<Viaje>>(HttpMethod.Get, "/api/viajes/activos") ?? [];

    public async Task<List<Viaje>> ObtenerViajesCanceladosAsync() =>
        await SendAndGetDataAsync<List<Viaje>>(HttpMethod.Get, "/api/viajes/cancelados") ?? [];

    public async Task<List<Reserva>> ObtenerPasajerosDelViajeAsync(int viajeId) =>
        await SendAndGetDataAsync<List<Reserva>>(HttpMethod.Get, $"/api/viajes/{viajeId}/pasajeros") ?? [];

    public async Task ReservarAsientoAsync(int viajeId, int pasajeroId, int numeroAsiento) =>
        await SendAndVerifyAsync(HttpMethod.Post, $"/api/viajes/{viajeId}/reservar", new { pasajeroId, numeroAsiento });

    public async Task CancelarReservaAsync(int reservaId) =>
        await SendAndVerifyAsync(HttpMethod.Post, $"/api/viajes/cancelar-reserva/{reservaId}");

    public async Task<(int pasajeros, int disponibles, decimal total)> ObtenerTotalesViajeAsync(int viajeId)
    {
        var response = await SendAsync<System.Text.Json.JsonElement>(HttpMethod.Get, $"/api/viajes/{viajeId}/totales");
        var data = response.Data;
        var pasajeros = data.GetProperty("pasajeros").GetInt32();
        var disponibles = data.GetProperty("disponibles").GetInt32();
        var total = data.GetProperty("total").GetDecimal();
        return (pasajeros, disponibles, total);
    }

    public async Task<List<Reserva>> ObtenerReservasPasajeroAsync(int pasajeroId) =>
        await SendAndGetDataAsync<List<Reserva>>(HttpMethod.Get, $"/api/viajes/reservas-pasajero/{pasajeroId}") ?? [];
}
