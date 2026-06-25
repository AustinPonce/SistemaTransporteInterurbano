using System.Net.Http.Json;
using System.Text.Json;
using SistemaTransporteInterurbano.Models.Entities;
using SistemaTransporteInterurbano.API.Models;

namespace SistemaTransporteInterurbano.WEB.Services;

public class ServicioClienteApi
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _urlBase;

    public ServicioClienteApi(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["ApiSettings:ApiKey"]!;
        _urlBase = configuration["ApiSettings:BaseUrl"]!;
    }

    private async Task<ApiRespuesta<T>> EnviarAsync<T>(HttpMethod method, string ruta, object? cuerpo = null)
    {
        try
        {
            var request = new HttpRequestMessage(method, $"{_urlBase}{ruta}");
            request.Headers.Add("X-API-Key", _apiKey);

            if (cuerpo != null)
                request.Content = JsonContent.Create(cuerpo);

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiRespuesta<T>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result == null)
            {
                return new ApiRespuesta<T> { Exitoso = false, Mensaje = "Error al procesar la respuesta de la API." };
            }

            return result;
        }
        catch (Exception ex)
        {
            return new ApiRespuesta<T>
            {
                Exitoso = false,
                Mensaje = $"Error de comunicación con el servidor: {ex.Message}"
            };
        }
    }

    private async Task<T?> EnviarYObtenerDatosAsync<T>(HttpMethod method, string ruta, object? cuerpo = null)
    {
        var response = await EnviarAsync<T>(method, ruta, cuerpo);

        if (response == null || !response.Exitoso)
            return default;

        return response.Datos;
    }

    private async Task EnviarYVerificarAsync(HttpMethod method, string ruta, object? cuerpo = null)
    {
        var response = await EnviarAsync<object>(method, ruta, cuerpo);

        if (!response.Exitoso)
            throw new Exception(response.Mensaje);
    }

    public async Task<List<Chofer>> ObtenerChoferesAsync(string? filtro = null)
    {
        var query = string.IsNullOrWhiteSpace(filtro) ? "" : $"?filtroNombre={Uri.EscapeDataString(filtro)}";
        return await EnviarYObtenerDatosAsync<List<Chofer>>(HttpMethod.Get, $"/api/choferes{query}") ?? [];
    }

    public async Task<Chofer?> ObtenerChoferPorIdAsync(int id) =>
        await EnviarYObtenerDatosAsync<Chofer>(HttpMethod.Get, $"/api/choferes/{id}");

    public async Task AgregarChoferAsync(string identificacion, string nombre, string apellidos, string correo) =>
        await EnviarYVerificarAsync(HttpMethod.Post, "/api/choferes", new { identificacion, nombre, apellidos, correoElectronico = correo });

    public async Task EditarChoferAsync(int id, string identificacion, string nombre, string apellidos) =>
        await EnviarYVerificarAsync(HttpMethod.Put, $"/api/choferes/{id}", new { identificacion, nombre, apellidos });

    public async Task EliminarChoferAsync(int id) =>
        await EnviarYVerificarAsync(HttpMethod.Delete, $"/api/choferes/{id}");

    public async Task<List<Pasajero>> ObtenerPasajerosAsync(string? filtro = null)
    {
        var query = string.IsNullOrWhiteSpace(filtro) ? "" : $"?filtroNombre={Uri.EscapeDataString(filtro)}";
        return await EnviarYObtenerDatosAsync<List<Pasajero>>(HttpMethod.Get, $"/api/pasajeros{query}") ?? [];
    }

    public async Task<Pasajero?> ObtenerPasajeroPorIdAsync(int id) =>
        await EnviarYObtenerDatosAsync<Pasajero>(HttpMethod.Get, $"/api/pasajeros/{id}");

    public async Task<Pasajero?> ObtenerPasajeroPorUsuarioIdAsync(int usuarioId) =>
        await EnviarYObtenerDatosAsync<Pasajero>(HttpMethod.Get, $"/api/pasajeros/por-usuario/{usuarioId}");

    public async Task AgregarPasajeroAsync(string identificacion, string nombre, string apellidos, string correo) =>
        await EnviarYVerificarAsync(HttpMethod.Post, "/api/pasajeros", new { identificacion, nombre, apellidos, correoElectronico = correo });

    public async Task EditarPasajeroAsync(int id, string identificacion, string nombre, string apellidos) =>
        await EnviarYVerificarAsync(HttpMethod.Put, $"/api/pasajeros/{id}", new { identificacion, nombre, apellidos });

    public async Task<List<Ruta>> ObtenerRutasAsync(string? filtro = null)
    {
        var query = string.IsNullOrWhiteSpace(filtro) ? "" : $"?filtro={Uri.EscapeDataString(filtro)}";
        return await EnviarYObtenerDatosAsync<List<Ruta>>(HttpMethod.Get, $"/api/rutas{query}") ?? [];
    }

    public async Task<Ruta?> ObtenerRutaPorIdAsync(int id) =>
        await EnviarYObtenerDatosAsync<Ruta>(HttpMethod.Get, $"/api/rutas/{id}");

    public async Task AgregarRutaAsync(string nombre, string origen, string destino, TimeSpan duracion, decimal precioBase) =>
        await EnviarYVerificarAsync(HttpMethod.Post, "/api/rutas", new { nombre, origen, destino, duracionEstimada = duracion.ToString(@"hh\:mm"), precioBase });

    public async Task EditarRutaAsync(int id, string nombre, string origen, string destino, TimeSpan duracion, decimal precioBase) =>
        await EnviarYVerificarAsync(HttpMethod.Put, $"/api/rutas/{id}", new { nombre, origen, destino, duracionEstimada = duracion.ToString(@"hh\:mm"), precioBase });

    public async Task<List<Unidad>> ObtenerUnidadesAsync() =>
        await EnviarYObtenerDatosAsync<List<Unidad>>(HttpMethod.Get, "/api/unidades") ?? [];

    public async Task<Unidad?> ObtenerUnidadPorIdAsync(int id) =>
        await EnviarYObtenerDatosAsync<Unidad>(HttpMethod.Get, $"/api/unidades/{id}");

    public async Task AgregarUnidadAsync(string placa, string modelo, int anio, int capacidad) =>
        await EnviarYVerificarAsync(HttpMethod.Post, "/api/unidades", new { placa, modelo, anioFabricacion = anio, capacidadPasajeros = capacidad });

    public async Task EditarUnidadAsync(int id, string placa, string modelo, int anio, int capacidad) =>
        await EnviarYVerificarAsync(HttpMethod.Put, $"/api/unidades/{id}", new { placa, modelo, anioFabricacion = anio, capacidadPasajeros = capacidad });

    public async Task<List<Viaje>> ObtenerViajesAsync(string? filtroRuta, DateTime? filtroFecha)
    {
        var query = $"?filtroRuta={Uri.EscapeDataString(filtroRuta ?? "")}&filtroFecha={filtroFecha?.ToString("yyyy-MM-dd")}";
        return await EnviarYObtenerDatosAsync<List<Viaje>>(HttpMethod.Get, $"/api/viajes{query}") ?? [];
    }

    public async Task<Viaje?> ObtenerViajePorIdAsync(int id) =>
        await EnviarYObtenerDatosAsync<Viaje>(HttpMethod.Get, $"/api/viajes/{id}");

    public async Task<Viaje?> ObtenerDetalleViajeAsync(int id) =>
        await EnviarYObtenerDatosAsync<Viaje>(HttpMethod.Get, $"/api/viajes/{id}/detalle");

    public async Task AgregarViajeAsync(int rutaId, int unidadId, int choferId, DateTime fechaSalida, DateTime fechaLlegada) =>
        await EnviarYVerificarAsync(HttpMethod.Post, "/api/viajes", new { rutaId, unidadId, choferId, fechaSalida, fechaLlegadaEstimada = fechaLlegada });

    public async Task EditarViajeAsync(int id, int rutaId, int unidadId, int choferId, DateTime fechaSalida, DateTime fechaLlegada) =>
        await EnviarYVerificarAsync(HttpMethod.Put, $"/api/viajes/{id}", new { rutaId, unidadId, choferId, fechaSalida, fechaLlegadaEstimada = fechaLlegada });

    public async Task CancelarViajeAsync(int id, string motivo) =>
        await EnviarYVerificarAsync(HttpMethod.Post, $"/api/viajes/{id}/cancelar", new { motivo });

    public async Task IniciarViajeAsync(int id) =>
        await EnviarYVerificarAsync(HttpMethod.Post, $"/api/viajes/{id}/iniciar");

    public async Task FinalizarViajeAsync(int id) =>
        await EnviarYVerificarAsync(HttpMethod.Post, $"/api/viajes/{id}/finalizar");

    public async Task<List<Viaje>> ObtenerViajesActivosAsync() =>
        await EnviarYObtenerDatosAsync<List<Viaje>>(HttpMethod.Get, "/api/viajes/activos") ?? [];

    public async Task<List<Viaje>> ObtenerViajesCanceladosAsync() =>
        await EnviarYObtenerDatosAsync<List<Viaje>>(HttpMethod.Get, "/api/viajes/cancelados") ?? [];

    public async Task<List<Reserva>> ObtenerPasajerosDelViajeAsync(int viajeId) =>
        await EnviarYObtenerDatosAsync<List<Reserva>>(HttpMethod.Get, $"/api/viajes/{viajeId}/pasajeros") ?? [];

    public async Task ReservarAsientoAsync(int viajeId, int pasajeroId, int numeroAsiento) =>
        await EnviarYVerificarAsync(HttpMethod.Post, $"/api/viajes/{viajeId}/reservar", new { pasajeroId, numeroAsiento });

    public async Task CancelarReservaAsync(int reservaId) =>
        await EnviarYVerificarAsync(HttpMethod.Post, $"/api/viajes/cancelar-reserva/{reservaId}");

    public async Task<(int pasajeros, int disponibles, decimal total)> ObtenerTotalesViajeAsync(int viajeId)
    {
        var response = await EnviarAsync<JsonElement>(HttpMethod.Get, $"/api/viajes/{viajeId}/totales");

        if (response == null || !response.Exitoso)
            return (0, 0, 0);

        var datos = response.Datos;
        var pasajeros = datos.GetProperty("pasajeros").GetInt32();
        var disponibles = datos.GetProperty("disponibles").GetInt32();
        var total = datos.GetProperty("total").GetDecimal();
        return (pasajeros, disponibles, total);
    }

    public async Task<List<Reserva>> ObtenerReservasPasajeroAsync(int pasajeroId) =>
        await EnviarYObtenerDatosAsync<List<Reserva>>(HttpMethod.Get, $"/api/viajes/reservas-pasajero/{pasajeroId}") ?? [];

    public async Task<(int usuarioId, string nombreUsuario, string rol, string mensaje)> IniciarSesionAsync(string nombreUsuario, string clave)
    {
        var response = await EnviarAsync<JsonElement>(HttpMethod.Post, "/api/autenticacion/iniciar-sesion", new { nombreUsuario, clave });

        if (response == null || !response.Exitoso)
            return (0, string.Empty, string.Empty, response?.Mensaje ?? "Error de comunicación con el servidor.");

        var datos = response.Datos;
        return (
            datos.GetProperty("usuarioId").GetInt32(),
            datos.GetProperty("nombreUsuario").GetString()!,
            datos.GetProperty("rol").GetString()!,
            string.Empty
        );
    }

    public async Task CambiarClaveAsync(string nombreUsuario, string claveActual, string nuevaClave) =>
        await EnviarYVerificarAsync(HttpMethod.Post, "/api/autenticacion/cambiar-clave", new { nombreUsuario, claveActual, nuevaClave });

    public async Task RecuperarAsync(string correo) =>
        await EnviarYVerificarAsync(HttpMethod.Post, "/api/autenticacion/recuperar", new { correo });

    public async Task ResetearAsync(string correo, string codigo, string nuevaClave) =>
        await EnviarYVerificarAsync(HttpMethod.Post, "/api/autenticacion/resetear", new { correo, codigo, nuevaClave });
}