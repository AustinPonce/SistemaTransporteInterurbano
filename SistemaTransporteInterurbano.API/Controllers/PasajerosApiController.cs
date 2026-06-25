using Microsoft.AspNetCore.Mvc;
using SistemaTransporteInterurbano.API.Helpers;
using SistemaTransporteInterurbano.API.Models;
using SistemaTransporteInterurbano.BL.Interfaces;
using SistemaTransporteInterurbano.Models.Entities;
using SistemaTransporteInterurbano.Models.ViewModels;

namespace SistemaTransporteInterurbano.API.Controllers;

[ApiController]
[Route("api/pasajeros")]
[ClaveApi]
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
            return Ok(ApiRespuesta<List<Pasajero>>.Exito(pasajeros));
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
            var pasajero = await _pasajeroService.ObtenerPorIdAsync(id);
            if (pasajero == null)
                return NotFound(ApiRespuesta<object>.Error("Pasajero no encontrado."));
            return Ok(ApiRespuesta<Pasajero>.Exito(pasajero));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiRespuesta<object>.Error(ex.Message));
        }
    }

    [HttpGet("por-usuario/{usuarioId}")]
    public async Task<IActionResult> ObtenerPorUsuarioId(int usuarioId)
    {
        try
        {
            var pasajero = await _pasajeroService.ObtenerPorUsuarioIdAsync(usuarioId);
            if (pasajero == null)
                return NotFound(ApiRespuesta<object>.Error("Pasajero no encontrado."));
            return Ok(ApiRespuesta<Pasajero>.Exito(pasajero));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiRespuesta<object>.Error(ex.Message));
        }
    }

    [HttpPost]
    public async Task<IActionResult> Agregar([FromBody] AgregarPasajeroViewModel vm)
    {
        try
        {
            await _pasajeroService.AgregarAsync(vm.Identificacion, vm.Nombre, vm.Apellidos, vm.CorreoElectronico);
            return Ok(ApiRespuesta<object>.Exito(null, "Pasajero registrado correctamente."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiRespuesta<object>.Error(ex.Message));
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Editar(int id, [FromBody] EditarPasajeroViewModel vm)
    {
        try
        {
            await _pasajeroService.EditarAsync(id, vm.Identificacion, vm.Nombre, vm.Apellidos);
            return Ok(ApiRespuesta<object>.Exito(null, "Pasajero actualizado correctamente."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiRespuesta<object>.Error(ex.Message));
        }
    }
}