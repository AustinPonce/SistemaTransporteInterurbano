using Microsoft.AspNetCore.Mvc;
using SistemaTransporteInterurbano.Models;
using SistemaTransporteInterurbano.Models.ViewModels;
using SistemaTransporteInterurbano.WEB.Services;

namespace SistemaTransporteInterurbano.WEB.Controllers;

public class ChoferController : Controller
{
    private readonly ServicioClienteApi _api;

    public ChoferController(ServicioClienteApi api)
    {
        _api = api;
    }

    private IActionResult? VerificarAdministrador()
    {
        var rol = HttpContext.Session.GetString("Rol");
        if (rol != Roles.Administrador)
            return RedirectToAction("IniciarSesion", "Autenticacion");
        return null;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? filtroNombre)
    {
        var redireccion = VerificarAdministrador();
        if (redireccion != null) return redireccion;

        var choferes = await _api.ObtenerChoferesAsync(filtroNombre);
        ViewBag.FiltroNombre = filtroNombre;
        return View(choferes);
    }

    [HttpGet]
    public IActionResult Agregar()
    {
        var redireccion = VerificarAdministrador();
        if (redireccion != null) return redireccion;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Agregar(AgregarChoferViewModel vm)
    {
        var redireccion = VerificarAdministrador();
        if (redireccion != null) return redireccion;

        try
        {
            if (!ModelState.IsValid)
                return View(vm);

            await _api.AgregarChoferAsync(vm.Identificacion, vm.Nombre, vm.Apellidos, vm.CorreoElectronico);

            TempData["MensajeExito"] = "Chofer registrado correctamente. Se envió la clave al correo.";
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
        var redireccion = VerificarAdministrador();
        if (redireccion != null) return redireccion;

        var chofer = await _api.ObtenerChoferPorIdAsync(id);

        if (chofer == null)
            return RedirectToAction("Index");

        var vm = new EditarChoferViewModel
        {
            ChoferId = chofer.ChoferId,
            Identificacion = chofer.Identificacion,
            Nombre = chofer.Nombre,
            Apellidos = chofer.Apellidos
        };

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Editar(EditarChoferViewModel vm)
    {
        var redireccion = VerificarAdministrador();
        if (redireccion != null) return redireccion;

        try
        {
            if (!ModelState.IsValid)
                return View(vm);

            await _api.EditarChoferAsync(vm.ChoferId, vm.Identificacion, vm.Nombre, vm.Apellidos);

            TempData["MensajeExito"] = "Chofer actualizado correctamente.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ViewBag.MensajeError = ex.Message;
            return View(vm);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(int id)
    {
        var redireccion = VerificarAdministrador();
        if (redireccion != null) return redireccion;

        try
        {
            await _api.EliminarChoferAsync(id);
            TempData["MensajeExito"] = "Chofer eliminado correctamente.";
        }
        catch (Exception ex)
        {
            TempData["MensajeError"] = ex.Message;
        }

        return RedirectToAction("Index");
    }
}
