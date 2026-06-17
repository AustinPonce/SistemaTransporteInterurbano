using Microsoft.AspNetCore.Mvc;
using SistemaTransporteInterurbano.BL.Interfaces;
using SistemaTransporteInterurbano.Models.Entities;
using SistemaTransporteInterurbano.WEB.Helpers;
using SistemaTransporteInterurbano.WEB.Models;

namespace SistemaTransporteInterurbano.WEB.Controllers.Api;

[ApiController]
[Route("api/choferes")]
[ApiKey]
public class ChoferesApiController : ControllerBase
{
    private readonly IChoferService _choferService;

    public ChoferesApiController(IChoferService choferService)
    {
        _choferService = choferService;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos(string? filtroNombre)
    {
        try
        {
            var choferes = await _choferService.ObtenerTodosAsync(filtroNombre);
            return Ok(ApiResponse<List<Chofer>>.Ok(choferes));
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
            var chofer = await _choferService.ObtenerPorIdAsync(id);
            if (chofer == null)
                return NotFound(ApiResponse<object>.Fail("Chofer no encontrado."));
            return Ok(ApiResponse<Chofer>.Ok(chofer));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPost]
    public async Task<IActionResult> Agregar([FromBody] Models.ViewModels.AgregarChoferViewModel vm)
    {
        try
        {
            await _choferService.AgregarAsync(vm.Identificacion, vm.Nombre, vm.Apellidos, vm.CorreoElectronico);
            return Ok(ApiResponse<object>.Ok(null, "Chofer registrado correctamente."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Editar(int id, [FromBody] Models.ViewModels.EditarChoferViewModel vm)
    {
        try
        {
            await _choferService.EditarAsync(id, vm.Identificacion, vm.Nombre, vm.Apellidos);
            return Ok(ApiResponse<object>.Ok(null, "Chofer actualizado correctamente."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        try
        {
            await _choferService.EliminarAsync(id);
            return Ok(ApiResponse<object>.Ok(null, "Chofer eliminado correctamente."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }
}
