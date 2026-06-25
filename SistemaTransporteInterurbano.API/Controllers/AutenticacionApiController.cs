using Microsoft.AspNetCore.Mvc;
using SistemaTransporteInterurbano.API.Helpers;
using SistemaTransporteInterurbano.API.Models;
using SistemaTransporteInterurbano.BL.Interfaces;

namespace SistemaTransporteInterurbano.API.Controllers;

[ApiController]
[Route("api/autenticacion")]
[ClaveApi]
public class AutenticacionApiController : ControllerBase
{
    private readonly IAutenticacionService _autenticacionService;

    public AutenticacionApiController(IAutenticacionService autenticacionService)
    {
        _autenticacionService = autenticacionService;
    }

    [HttpGet("generar-hash-temporal")]
    public IActionResult GenerarHashTemporal(string clave)
    {
        var hash = BCrypt.Net.BCrypt.HashPassword(clave);
        return Ok(hash);
    }

    [HttpPost("iniciar-sesion")]
    public async Task<IActionResult> IniciarSesion([FromBody] InicioSesionRequest solicitud)
    {
        try
        {
            var usuario = await _autenticacionService.AutenticarUsuarioPorNombreYClave(
                solicitud.NombreUsuario, solicitud.Clave);

            return Ok(ApiRespuesta<object>.Exito(new
            {
                usuarioId = usuario!.UsuarioId,
                nombreUsuario = usuario.NombreUsuario,
                rol = usuario.Rol!.Nombre
            }));
        }
        catch (Exception error)
        {
            return BadRequest(ApiRespuesta<object>.Error(error.Message));
        }
    }

    [HttpPost("cambiar-clave")]
    public async Task<IActionResult> CambiarClave([FromBody] CambiarClaveRequest solicitud)
    {
        try
        {
            await _autenticacionService.CambiarClaveDeUsuario(
                solicitud.NombreUsuario, solicitud.ClaveActual, solicitud.NuevaClave);
            return Ok(ApiRespuesta<object>.Exito(null, "Clave actualizada correctamente."));
        }
        catch (Exception error)
        {
            return BadRequest(ApiRespuesta<object>.Error(error.Message));
        }
    }

    [HttpPost("recuperar")]
    public async Task<IActionResult> Recuperar([FromBody] RecuperarRequest solicitud)
    {
        try
        {
            await _autenticacionService.IniciarRecuperacionPorCorreoAsync(solicitud.Correo);
            return Ok(ApiRespuesta<object>.Exito(null, "Código enviado al correo."));
        }
        catch (Exception error)
        {
            return BadRequest(ApiRespuesta<object>.Error(error.Message));
        }
    }

    [HttpPost("resetear")]
    public async Task<IActionResult> Resetear([FromBody] ResetearRequest solicitud)
    {
        try
        {
            await _autenticacionService.ResetearClaveConCodigoAsync(
                solicitud.Correo, solicitud.Codigo, solicitud.NuevaClave);
            return Ok(ApiRespuesta<object>.Exito(null, "Clave restablecida correctamente."));
        }
        catch (Exception error)
        {
            return BadRequest(ApiRespuesta<object>.Error(error.Message));
        }
    }
}

public class InicioSesionRequest
{
    public string NombreUsuario { get; set; } = string.Empty;
    public string Clave { get; set; } = string.Empty;
}

public class CambiarClaveRequest
{
    public string NombreUsuario { get; set; } = string.Empty;
    public string ClaveActual { get; set; } = string.Empty;
    public string NuevaClave { get; set; } = string.Empty;
}

public class RecuperarRequest
{
    public string Correo { get; set; } = string.Empty;
}

public class ResetearRequest
{
    public string Correo { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public string NuevaClave { get; set; } = string.Empty;
}