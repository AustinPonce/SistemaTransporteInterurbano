using Microsoft.AspNetCore.Mvc;
using SistemaTransporteInterurbano.API.Helpers;
using SistemaTransporteInterurbano.API.Models;
using SistemaTransporteInterurbano.BL.Interfaces;
using SistemaTransporteInterurbano.Models.Entities;
using SistemaTransporteInterurbano.Models.ViewModels;

namespace SistemaTransporteInterurbano.API.Controllers;

[ApiController]
[Route("api/rutas")]
[ClaveApi]
public class RutasApiController : ControllerBase
{
    private readonly IRutaService _rutaService;

    public RutasApiController(IRutaService rutaService)
    {
        _rutaService = rutaService;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodas(string? filtro)
    {
        try
        {
            var rutas = await _rutaService.ObtenerTodasAsync(filtro);
            return Ok(ApiRespuesta<List<Ruta>>.Exito(rutas));
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
            var ruta = await _rutaService.ObtenerPorIdAsync(id);
            if (ruta == null)
                return NotFound(ApiRespuesta<object>.Error("Ruta no encontrada."));
            return Ok(ApiRespuesta<Ruta>.Exito(ruta));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiRespuesta<object>.Error(ex.Message));
        }
    }

    [HttpPost]
    public async Task<IActionResult> Agregar([FromBody] AgregarRutaViewModel vm)
    {
        try
        {
            if (!TimeSpan.TryParseExact(vm.DuracionEstimada, @"hh\:mm", null, out var duracion))
                return BadRequest(ApiRespuesta<object>.Error("El formato de duración es inválido. Use hh:mm."));

            await _rutaService.AgregarAsync(vm.Nombre, vm.Origen, vm.Destino, duracion, vm.PrecioBase);
            return Ok(ApiRespuesta<object>.Exito(null, "Ruta registrada correctamente."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiRespuesta<object>.Error(ex.Message));
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Editar(int id, [FromBody] EditarRutaViewModel vm)
    {
        try
        {
            if (!TimeSpan.TryParseExact(vm.DuracionEstimada, @"hh\:mm", null, out var duracion))
                return BadRequest(ApiRespuesta<object>.Error("El formato de duración es inválido. Use hh:mm."));

            await _rutaService.EditarAsync(id, vm.Nombre, vm.Origen, vm.Destino, duracion, vm.PrecioBase);
            return Ok(ApiRespuesta<object>.Exito(null, "Ruta actualizada correctamente."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiRespuesta<object>.Error(ex.Message));
        }
    }
}