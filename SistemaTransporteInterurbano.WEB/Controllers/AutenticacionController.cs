using Microsoft.AspNetCore.Mvc;
using SistemaTransporteInterurbano.WEB.Models.ViewModels;
using SistemaTransporteInterurbano.WEB.Services;
using SistemaTransporteInterurbano.Models;

namespace SistemaTransporteInterurbano.WEB.Controllers;

public class AutenticacionController : Controller
{
    private readonly ServicioClienteApi _api;

    public AutenticacionController(ServicioClienteApi api)
    {
        _api = api;
    }

    [HttpGet]
    public IActionResult IniciarSesion()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> IniciarSesion(LoginViewModel viewModel)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var (usuarioId, nombreUsuario, rol) = await _api.IniciarSesionAsync(
                viewModel.NombreUsuario, viewModel.Clave);

            HttpContext.Session.SetString("NombreUsuario", nombreUsuario);
            HttpContext.Session.SetString("Rol", rol);
            HttpContext.Session.SetInt32("UsuarioId", usuarioId);

            TempData["MensajeExito"] = "Inicio de sesión exitoso.";

            return rol switch
            {
                Roles.Administrador => RedirectToAction("Index", "Home"),
                Roles.Chofer => RedirectToAction("Index", "Home"),
                Roles.Pasajero => RedirectToAction("Index", "MisViajes"),
                _ => RedirectToAction("Index", "Home")
            };
        }
        catch (Exception ex)
        {
            if (ex.Message == "Debe cambiar la contraseña antes de continuar.")
            {
                TempData["MensajeExito"] = "Debe cambiar la contraseña para continuar.";
                return RedirectToAction("CambiarClave");
            }

            ViewBag.MensajeError = ex.Message;
            return View(viewModel);
        }
    }

    [HttpGet]
    public IActionResult CambiarClave()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CambiarClave(CambiarClaveViewModel viewModel)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            await _api.CambiarClaveAsync(
                viewModel.NombreUsuario,
                viewModel.ClaveActual,
                viewModel.NuevaClave);

            TempData["MensajeExito"] = "La clave fue actualizada correctamente.";
            return RedirectToAction("IniciarSesion");
        }
        catch (Exception ex)
        {
            ViewBag.MensajeError = ex.Message;
            return View(viewModel);
        }
    }

    [HttpGet]
    public IActionResult Recuperar()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Recuperar(ForgotPasswordViewModel vm)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(vm);

            await _api.RecuperarAsync(vm.Correo);
            TempData["MensajeExito"] = "Se envió un código a su correo.";
            return RedirectToAction("Resetear");
        }
        catch (Exception ex)
        {
            ViewBag.MensajeError = ex.Message;
            return View(vm);
        }
    }

    [HttpGet]
    public IActionResult Resetear()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Resetear(ResetPasswordViewModel vm)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(vm);

            await _api.ResetearAsync(vm.Correo, vm.Codigo, vm.NuevaClave);
            TempData["MensajeExito"] = "La contraseña fue restablecida.";
            return RedirectToAction("IniciarSesion");
        }
        catch (Exception ex)
        {
            ViewBag.MensajeError = ex.Message;
            return View(vm);
        }
    }

    public IActionResult CerrarSesion()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("IniciarSesion");
    }
}