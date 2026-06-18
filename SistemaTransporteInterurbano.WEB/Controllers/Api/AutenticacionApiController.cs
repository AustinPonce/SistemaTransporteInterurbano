using Microsoft.AspNetCore.Mvc;
using SistemaTransporteInterurbano.BL.Interfaces;
using SistemaTransporteInterurbano.Models;

namespace SistemaTransporteInterurbano.WEB.Controllers.Api;

[ApiController]
[Route("api/auth")]
public class AutenticacionApiController : ControllerBase
{
    private readonly IAutenticacionService _autenticacionService;
    private readonly IPasajeroService _pasajeroService;

    public AutenticacionApiController(
        IAutenticacionService autenticacionService,
        IPasajeroService pasajeroService)
    {
        _autenticacionService = autenticacionService;
        _pasajeroService = pasajeroService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginApiRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.NombreUsuario) ||
                string.IsNullOrWhiteSpace(request.Clave))
            {
                return BadRequest(new
                {
                    mensaje = "Debe ingresar nombre de usuario y clave."
                });
            }

            var usuario = await _autenticacionService
                .AutenticarUsuarioPorNombreYClave(
                    request.NombreUsuario,
                    request.Clave);

            if (usuario == null)
            {
                return Unauthorized(new
                {
                    mensaje = "Credenciales incorrectas."
                });
            }

            if (usuario.Rol?.Nombre != Roles.Pasajero)
            {
                return Unauthorized(new
                {
                    mensaje = "La aplicación móvil es exclusiva para pasajeros."
                });
            }

            var pasajero = await _pasajeroService.ObtenerPorUsuarioIdAsync(usuario.UsuarioId);

            if (pasajero == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el pasajero asociado al usuario."
                });
            }

            return Ok(new LoginApiResponse
            {
                UsuarioId = usuario.UsuarioId,
                PasajeroId = pasajero.PasajeroId,
                NombreUsuario = usuario.NombreUsuario,
                Rol = usuario.Rol.Nombre,
                NombreCompleto = $"{pasajero.Nombre} {pasajero.Apellidos}"
            });
        }
        catch (Exception ex)
        {
            return Unauthorized(new
            {
                mensaje = ex.Message
            });
        }
    }
}

public class LoginApiRequest
{
    public string NombreUsuario { get; set; } = string.Empty;
    public string Clave { get; set; } = string.Empty;
}

public class LoginApiResponse
{
    public int UsuarioId { get; set; }
    public int PasajeroId { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
}