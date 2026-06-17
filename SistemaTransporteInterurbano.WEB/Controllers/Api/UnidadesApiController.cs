using Microsoft.AspNetCore.Mvc;
using SistemaTransporteInterurbano.BL.Interfaces;
using SistemaTransporteInterurbano.Models.Entities;
using SistemaTransporteInterurbano.WEB.Helpers;
using SistemaTransporteInterurbano.WEB.Models;

namespace SistemaTransporteInterurbano.WEB.Controllers.Api;

[ApiController]
[Route("api/unidades")]
[ApiKey]
public class UnidadesApiController : ControllerBase
{
    private readonly IUnidadService _unidadService;

    public UnidadesApiController(IUnidadService unidadService)
    {
        _unidadService = unidadService;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodas()
    {
        try
        {
            var unidades = await _unidadService.ObtenerTodasAsync();
            return Ok(ApiResponse<List<Unidad>>.Ok(unidades));
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
            var unidad = await _unidadService.ObtenerPorIdAsync(id);
            if (unidad == null)
                return NotFound(ApiResponse<object>.Fail("Unidad no encontrada."));
            return Ok(ApiResponse<Unidad>.Ok(unidad));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPost]
    public async Task<IActionResult> Agregar([FromBody] Models.ViewModels.AgregarUnidadViewModel vm)
    {
        try
        {
            await _unidadService.AgregarAsync(vm.Placa, vm.Modelo, vm.AnioFabricacion, vm.CapacidadPasajeros);
            return Ok(ApiResponse<object>.Ok(null, "Unidad registrada correctamente."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Editar(int id, [FromBody] Models.ViewModels.EditarUnidadViewModel vm)
    {
        try
        {
            await _unidadService.EditarAsync(id, vm.Placa, vm.Modelo, vm.AnioFabricacion, vm.CapacidadPasajeros);
            return Ok(ApiResponse<object>.Ok(null, "Unidad actualizada correctamente."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }
}
