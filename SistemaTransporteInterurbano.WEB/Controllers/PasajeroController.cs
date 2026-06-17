using Microsoft.AspNetCore.Mvc;
using SistemaTransporteInterurbano.Models;
using SistemaTransporteInterurbano.WEB.Models.ViewModels;
using SistemaTransporteInterurbano.WEB.Services;

namespace SistemaTransporteInterurbano.WEB.Controllers;

public class PasajeroController : Controller
{
    private readonly ApiClientService _api;

    public PasajeroController(ApiClientService api)
    {
        _api = api;
    }

    private IActionResult? VerificarChofer()
    {
        var rol = HttpContext.Session.GetString("Rol");
        if (rol != Roles.Chofer)
            return RedirectToAction("IniciarSesion", "Autenticacion");
        return null;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? filtroNombre)
    {
        var redireccion = VerificarChofer();
        if (redireccion != null) return redireccion;

        var pasajeros = await _api.ObtenerPasajerosAsync(filtroNombre);
        ViewBag.FiltroNombre = filtroNombre;
        return View(pasajeros);
    }

    [HttpGet]
    public IActionResult Agregar()
    {
        var redireccion = VerificarChofer();
        if (redireccion != null) return redireccion;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Agregar(AgregarPasajeroViewModel vm)
    {
        var redireccion = VerificarChofer();
        if (redireccion != null) return redireccion;

        try
        {
            if (!ModelState.IsValid)
                return View(vm);

            await _api.AgregarPasajeroAsync(vm.Identificacion, vm.Nombre, vm.Apellidos, vm.CorreoElectronico);

            TempData["MensajeExito"] = "Pasajero registrado correctamente. Se envió la clave al correo.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ViewBag.MensajeError = ex.Message;
            return View(vm);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var redireccion = VerificarChofer();
        if (redireccion != null) return redireccion;

        var pasajero = await _api.ObtenerPasajeroPorIdAsync(id);

        if (pasajero == null)
            return RedirectToAction("Index");

        var vm = new EditarPasajeroViewModel
        {
            PasajeroId = pasajero.PasajeroId,
            Identificacion = pasajero.Identificacion,
            Nombre = pasajero.Nombre,
            Apellidos = pasajero.Apellidos
        };

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Editar(EditarPasajeroViewModel vm)
    {
        var redireccion = VerificarChofer();
        if (redireccion != null) return redireccion;

        try
        {
            if (!ModelState.IsValid)
                return View(vm);

            await _api.EditarPasajeroAsync(vm.PasajeroId, vm.Identificacion, vm.Nombre, vm.Apellidos);

            TempData["MensajeExito"] = "Pasajero actualizado correctamente.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ViewBag.MensajeError = ex.Message;
            return View(vm);
        }
    }
}
