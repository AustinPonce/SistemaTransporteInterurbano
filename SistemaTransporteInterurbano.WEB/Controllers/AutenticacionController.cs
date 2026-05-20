using Microsoft.AspNetCore.Mvc;
using SistemaTransporteInterurbano.BL.Interfaces;
using SistemaTransporteInterurbano.WEB.Models.ViewModels;

namespace SistemaTransporteInterurbano.WEB.Controllers;

public class AutenticacionController : Controller
{
    private readonly IAutenticacionService
        _autenticacionService;

    private readonly INotificacionCorreoService _notificacionCorreoService;

    public AutenticacionController(IAutenticacionService autenticacionService, INotificacionCorreoService notificacionCorreoService)
    {
        _autenticacionService = autenticacionService;
        _notificacionCorreoService = notificacionCorreoService;
    }

    [HttpGet]
    public IActionResult IniciarSesion()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult>
        IniciarSesion(LoginViewModel viewModel)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var usuario =
                await _autenticacionService
                    .AutenticarUsuarioPorNombreYClave(
                        viewModel.NombreUsuario,
                        viewModel.Clave);

            TempData["MensajeExito"] =
                "Inicio de sesión exitoso.";

            return RedirectToAction(
                "Index",
                "Home");
        }
            catch (Exception ex)
          {
            if (ex.Message ==
                "Debe cambiar la contraseña antes de continuar.")
            {
                TempData["MensajeExito"] =
                    "Debe cambiar la contraseña para continuar.";

                return RedirectToAction(
                    "CambiarClave");
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
    public async Task<IActionResult>
        CambiarClave(
            CambiarClaveViewModel viewModel)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            await _autenticacionService
                .CambiarClaveDeUsuario(
                    viewModel.NombreUsuario,
                    viewModel.ClaveActual,
                    viewModel.NuevaClave);

            TempData["MensajeExito"] =
                "La clave fue actualizada correctamente.";

            return RedirectToAction(
                "IniciarSesion");
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

            await _autenticacionService.IniciarRecuperacionPorCorreoAsync(vm.Correo);

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

            await _autenticacionService.ResetearClaveConCodigoAsync(vm.Correo, vm.Codigo, vm.NuevaClave);

            TempData["MensajeExito"] = "La contraseña fue restablecida.";

            return RedirectToAction("IniciarSesion");
        }
        catch (Exception ex)
        {
            ViewBag.MensajeError = ex.Message;
            return View(vm);
        }
    }
}