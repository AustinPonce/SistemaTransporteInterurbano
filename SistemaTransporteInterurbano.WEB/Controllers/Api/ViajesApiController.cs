using Microsoft.AspNetCore.Mvc;
using SistemaTransporteInterurbano.BL.Interfaces;
using SistemaTransporteInterurbano.Models.Entities;
using SistemaTransporteInterurbano.WEB.Helpers;
using SistemaTransporteInterurbano.WEB.Models;

namespace SistemaTransporteInterurbano.WEB.Controllers.Api;

[ApiController]
[Route("api/viajes")]
[ClaveApi]
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
            return Ok(ApiRespuesta<List<Viaje>>.Exito(viajes));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiRespuesta<object>.Error(ex.Message));
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        try
        {
            var viaje = await _viajeService.ObtenerPorIdAsync(id);
            if (viaje == null)
                return NotFound(ApiRespuesta<object>.Error("Viaje no encontrado."));
            return Ok(ApiRespuesta<Viaje>.Exito(viaje));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiRespuesta<object>.Error(ex.Message));
        }
    }

    [HttpGet("{id}/detalle")]
    public async Task<IActionResult> ObtenerDetalle(int id)
    {
        try
        {
            var viaje = await _viajeService.ObtenerDetalleAsync(id);
            if (viaje == null)
                return NotFound(ApiRespuesta<object>.Error("Viaje no encontrado."));
            return Ok(ApiRespuesta<Viaje>.Exito(viaje));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiRespuesta<object>.Error(ex.Message));
        }
    }

    [HttpGet("activos")]
    public async Task<IActionResult> ObtenerActivos()
    {
        try
        {
            var viajes = await _viajeService.ObtenerActivosAsync();
            return Ok(ApiRespuesta<List<Viaje>>.Exito(viajes));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiRespuesta<object>.Error(ex.Message));
        }
    }

    [HttpGet("cancelados")]
    public async Task<IActionResult> ObtenerCancelados()
    {
        try
        {
            var viajes = await _viajeService.ObtenerCanceladosAsync();
            return Ok(ApiRespuesta<List<Viaje>>.Exito(viajes));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiRespuesta<object>.Error(ex.Message));
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
            return Ok(ApiRespuesta<object>.Exito(null, "Viaje registrado correctamente."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiRespuesta<object>.Error(ex.Message));
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
            return Ok(ApiRespuesta<object>.Exito(null, "Viaje actualizado correctamente."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiRespuesta<object>.Error(ex.Message));
        }
    }

    [HttpPost("{id}/cancelar")]
    public async Task<IActionResult> Cancelar(int id, [FromBody] CancelarRequest request)
    {
        try
        {
            await _viajeService.CancelarAsync(id, request.Motivo, _correoService);
            return Ok(ApiRespuesta<object>.Exito(null, "Viaje cancelado correctamente."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiRespuesta<object>.Error(ex.Message));
        }
    }

    [HttpPost("{id}/iniciar")]
    public async Task<IActionResult> Iniciar(int id)
    {
        try
        {
            await _viajeService.IniciarAsync(id);
            return Ok(ApiRespuesta<object>.Exito(null, "Viaje iniciado."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiRespuesta<object>.Error(ex.Message));
        }
    }

    [HttpPost("{id}/finalizar")]
    public async Task<IActionResult> Finalizar(int id)
    {
        try
        {
            await _viajeService.FinalizarViajeAsync(id);
            return Ok(ApiRespuesta<object>.Exito(null, "Viaje finalizado."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiRespuesta<object>.Error(ex.Message));
        }
    }

    [HttpGet("{id}/pasajeros")]
    public async Task<IActionResult> ObtenerPasajeros(int id)
    {
        try
        {
            var reservas = await _viajeService.ObtenerPasajerosAsync(id);
            return Ok(ApiRespuesta<List<Reserva>>.Exito(reservas));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiRespuesta<object>.Error(ex.Message));
        }
    }

    [HttpPost("{id}/reservar")]
    public async Task<IActionResult> Reservar(int id, [FromBody] ReservarRequest request)
    {
        try
        {
            await _viajeService.ReservarAsientoAsync(id, request.PasajeroId, request.NumeroAsiento);
            return Ok(ApiRespuesta<object>.Exito(null, "Reserva registrada correctamente."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiRespuesta<object>.Error(ex.Message));
        }
    }

    [HttpPost("cancelar-reserva/{reservaId}")]
    public async Task<IActionResult> CancelarReserva(int reservaId)
    {
        try
        {
            await _viajeService.CancelarReservaAsync(reservaId);
            return Ok(ApiRespuesta<object>.Exito(null, "Reserva cancelada correctamente."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiRespuesta<object>.Error(ex.Message));
        }
    }

    [HttpGet("{id}/totales")]
    public async Task<IActionResult> ObtenerTotales(int id)
    {
        try
        {
            var totales = await _viajeService.ObtenerTotalesAsync(id);
            return Ok(ApiRespuesta<object>.Exito(new { totales.pasajeros, totales.disponibles, totales.total }));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiRespuesta<object>.Error(ex.Message));
        }
    }

    [HttpGet("reservas-pasajero/{pasajeroId}")]
    public async Task<IActionResult> ObtenerReservasPasajero(int pasajeroId)
    {
        try
        {
            var reservas = await _viajeService.ObtenerReservasPasajeroAsync(pasajeroId);
            return Ok(ApiRespuesta<List<Reserva>>.Exito(reservas));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiRespuesta<object>.Error(ex.Message));
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
