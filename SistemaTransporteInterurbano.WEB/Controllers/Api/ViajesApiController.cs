using Microsoft.AspNetCore.Mvc;
using SistemaTransporteInterurbano.BL.Interfaces;
using SistemaTransporteInterurbano.Models.Entities;
using SistemaTransporteInterurbano.WEB.Helpers;
using SistemaTransporteInterurbano.WEB.Models;

namespace SistemaTransporteInterurbano.WEB.Controllers.Api;

[ApiController]
[Route("api/viajes")]
[ApiKey]
public class ViajesApiController : ControllerBase
{
    private readonly IViajeService _viajeService;
    private readonly INotificacionCorreoService _correoService;

    public ViajesApiController(IViajeService viajeService, INotificacionCorreoService correoService)
    {
        _viajeService = viajeService;
        _correoService = correoService;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos(string? filtroRuta, DateTime? filtroFecha)
    {
        try
        {
            var viajes = await _viajeService.ObtenerTodosAsync(filtroRuta, filtroFecha);
            return Ok(ApiResponse<List<Viaje>>.Ok(viajes));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        try
        {
            var viaje = await _viajeService.ObtenerPorIdAsync(id);
            if (viaje == null)
                return NotFound(ApiResponse<object>.Fail("Viaje no encontrado."));
            return Ok(ApiResponse<Viaje>.Ok(viaje));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpGet("{id}/detalle")]
    public async Task<IActionResult> ObtenerDetalle(int id)
    {
        try
        {
            var viaje = await _viajeService.ObtenerDetalleAsync(id);
            if (viaje == null)
                return NotFound(ApiResponse<object>.Fail("Viaje no encontrado."));
            return Ok(ApiResponse<Viaje>.Ok(viaje));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpGet("activos")]
    public async Task<IActionResult> ObtenerActivos()
    {
        try
        {
            var viajes = await _viajeService.ObtenerActivosAsync();
            return Ok(ApiResponse<List<Viaje>>.Ok(viajes));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpGet("cancelados")]
    public async Task<IActionResult> ObtenerCancelados()
    {
        try
        {
            var viajes = await _viajeService.ObtenerCanceladosAsync();
            return Ok(ApiResponse<List<Viaje>>.Ok(viajes));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPost]
    public async Task<IActionResult> Agregar([FromBody] Models.ViewModels.AgregarViajeViewModel vm)
    {
        try
        {
            await _viajeService.AgregarAsync(
                vm.RutaId!.Value, vm.UnidadId!.Value, vm.ChoferId!.Value,
                vm.FechaSalida!.Value, vm.FechaLlegadaEstimada!.Value);
            return Ok(ApiResponse<object>.Ok(null, "Viaje registrado correctamente."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Editar(int id, [FromBody] Models.ViewModels.EditarViajeViewModel vm)
    {
        try
        {
            await _viajeService.EditarAsync(
                id, vm.RutaId!.Value, vm.UnidadId!.Value, vm.ChoferId!.Value,
                vm.FechaSalida!.Value, vm.FechaLlegadaEstimada!.Value);
            return Ok(ApiResponse<object>.Ok(null, "Viaje actualizado correctamente."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPost("{id}/cancelar")]
    public async Task<IActionResult> Cancelar(int id, [FromBody] CancelarRequest request)
    {
        try
        {
            await _viajeService.CancelarAsync(id, request.Motivo, _correoService);
            return Ok(ApiResponse<object>.Ok(null, "Viaje cancelado correctamente."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPost("{id}/iniciar")]
    public async Task<IActionResult> Iniciar(int id)
    {
        try
        {
            await _viajeService.IniciarAsync(id);
            return Ok(ApiResponse<object>.Ok(null, "Viaje iniciado."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPost("{id}/finalizar")]
    public async Task<IActionResult> Finalizar(int id)
    {
        try
        {
            await _viajeService.FinalizarViajeAsync(id);
            return Ok(ApiResponse<object>.Ok(null, "Viaje finalizado."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpGet("{id}/pasajeros")]
    public async Task<IActionResult> ObtenerPasajeros(int id)
    {
        try
        {
            var reservas = await _viajeService.ObtenerPasajerosAsync(id);
            return Ok(ApiResponse<List<Reserva>>.Ok(reservas));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPost("{id}/reservar")]
    public async Task<IActionResult> Reservar(int id, [FromBody] ReservarRequest request)
    {
        try
        {
            await _viajeService.ReservarAsientoAsync(id, request.PasajeroId, request.NumeroAsiento);
            return Ok(ApiResponse<object>.Ok(null, "Reserva registrada correctamente."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPost("cancelar-reserva/{reservaId}")]
    public async Task<IActionResult> CancelarReserva(int reservaId)
    {
        try
        {
            await _viajeService.CancelarReservaAsync(reservaId);
            return Ok(ApiResponse<object>.Ok(null, "Reserva cancelada correctamente."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpGet("{id}/totales")]
    public async Task<IActionResult> ObtenerTotales(int id)
    {
        try
        {
            var totales = await _viajeService.ObtenerTotalesAsync(id);
            return Ok(ApiResponse<object>.Ok(new { totales.pasajeros, totales.disponibles, totales.total }));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpGet("reservas-pasajero/{pasajeroId}")]
    public async Task<IActionResult> ObtenerReservasPasajero(int pasajeroId)
    {
        try
        {
            var reservas = await _viajeService.ObtenerReservasPasajeroAsync(pasajeroId);
            return Ok(ApiResponse<List<Reserva>>.Ok(reservas));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    public class ReservarRequest
    {
        public int PasajeroId { get; set; }
        public int NumeroAsiento { get; set; }
    }

    public class CancelarRequest
    {
        public string Motivo { get; set; } = string.Empty;
    }
}
