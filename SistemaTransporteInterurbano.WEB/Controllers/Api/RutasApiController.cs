using Microsoft.AspNetCore.Mvc;
using SistemaTransporteInterurbano.BL.Interfaces;
using SistemaTransporteInterurbano.Models.Entities;
using SistemaTransporteInterurbano.WEB.Helpers;
using SistemaTransporteInterurbano.WEB.Models;

namespace SistemaTransporteInterurbano.WEB.Controllers.Api;

[ApiController]
[Route("api/rutas")]
[ApiKey]
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
            return Ok(ApiResponse<List<Ruta>>.Ok(rutas));
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
            var ruta = await _rutaService.ObtenerPorIdAsync(id);
            if (ruta == null)
                return NotFound(ApiResponse<object>.Fail("Ruta no encontrada."));
            return Ok(ApiResponse<Ruta>.Ok(ruta));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPost]
    public async Task<IActionResult> Agregar([FromBody] Models.ViewModels.AgregarRutaViewModel vm)
    {
        try
        {
            if (!TimeSpan.TryParseExact(vm.DuracionEstimada, @"hh\:mm", null, out var duracion))
                return BadRequest(ApiResponse<object>.Fail("El formato de duración es inválido. Use hh:mm."));

            await _rutaService.AgregarAsync(vm.Nombre, vm.Origen, vm.Destino, duracion, vm.PrecioBase);
            return Ok(ApiResponse<object>.Ok(null, "Ruta registrada correctamente."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Editar(int id, [FromBody] Models.ViewModels.EditarRutaViewModel vm)
    {
        try
        {
            if (!TimeSpan.TryParseExact(vm.DuracionEstimada, @"hh\:mm", null, out var duracion))
                return BadRequest(ApiResponse<object>.Fail("El formato de duración es inválido. Use hh:mm."));

            await _rutaService.EditarAsync(id, vm.Nombre, vm.Origen, vm.Destino, duracion, vm.PrecioBase);
            return Ok(ApiResponse<object>.Ok(null, "Ruta actualizada correctamente."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }
}
