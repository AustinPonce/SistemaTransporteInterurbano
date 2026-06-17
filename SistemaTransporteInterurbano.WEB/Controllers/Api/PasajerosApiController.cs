using Microsoft.AspNetCore.Mvc;
using SistemaTransporteInterurbano.BL.Interfaces;
using SistemaTransporteInterurbano.Models.Entities;
using SistemaTransporteInterurbano.WEB.Helpers;
using SistemaTransporteInterurbano.WEB.Models;

namespace SistemaTransporteInterurbano.WEB.Controllers.Api;

[ApiController]
[Route("api/pasajeros")]
[ApiKey]
public class PasajerosApiController : ControllerBase
{
    private readonly IPasajeroService _pasajeroService;

    public PasajerosApiController(IPasajeroService pasajeroService)
    {
        _pasajeroService = pasajeroService;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos(string? filtroNombre)
    {
        try
        {
            var pasajeros = await _pasajeroService.ObtenerTodosAsync(filtroNombre);
            return Ok(ApiResponse<List<Pasajero>>.Ok(pasajeros));
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
            var pasajero = await _pasajeroService.ObtenerPorIdAsync(id);
            if (pasajero == null)
                return NotFound(ApiResponse<object>.Fail("Pasajero no encontrado."));
            return Ok(ApiResponse<Pasajero>.Ok(pasajero));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpGet("por-usuario/{usuarioId}")]
    public async Task<IActionResult> ObtenerPorUsuarioId(int usuarioId)
    {
        try
        {
            var pasajero = await _pasajeroService.ObtenerPorUsuarioIdAsync(usuarioId);
            if (pasajero == null)
                return NotFound(ApiResponse<object>.Fail("Pasajero no encontrado."));
            return Ok(ApiResponse<Pasajero>.Ok(pasajero));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPost]
    public async Task<IActionResult> Agregar([FromBody] Models.ViewModels.AgregarPasajeroViewModel vm)
    {
        try
        {
            await _pasajeroService.AgregarAsync(vm.Identificacion, vm.Nombre, vm.Apellidos, vm.CorreoElectronico);
            return Ok(ApiResponse<object>.Ok(null, "Pasajero registrado correctamente."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Editar(int id, [FromBody] Models.ViewModels.EditarPasajeroViewModel vm)
    {
        try
        {
            await _pasajeroService.EditarAsync(id, vm.Identificacion, vm.Nombre, vm.Apellidos);
            return Ok(ApiResponse<object>.Ok(null, "Pasajero actualizado correctamente."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }
}
