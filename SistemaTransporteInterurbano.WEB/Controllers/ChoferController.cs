using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SistemaTransporteInterurbano.BL.Interfaces;
using SistemaTransporteInterurbano.WEB.Models.ViewModels;
using SistemaTransporteInterurbano.Models;

namespace SistemaTransporteInterurbano.WEB.Controllers;

public class ChoferController : Controller
{
    private readonly IChoferService _choferService;

    public ChoferController(IChoferService choferService)
    {
        _choferService = choferService;
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

        var choferes = await _choferService.ObtenerTodosAsync(filtroNombre);
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

            await _choferService.AgregarAsync(
                vm.Identificacion,
                vm.Nombre,
                vm.Apellidos,
                vm.CorreoElectronico);

            TempData["MensajeExito"] = "Chofer registrado correctamente. Se envió la clave al correo.";
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
            if (_choferService is SistemaTransporteInterurbano.BL.Services.ChoferService servicioConcreto)
            {
                await servicioConcreto.EliminarAsync(id);
            }
            else
            {
                throw new Exception("Operación de eliminación no disponible.");
            }
            TempData["MensajeExito"] = "Chofer eliminado correctamente.";
        }
        catch (Exception ex)
        {
            TempData["MensajeError"] = ex.Message;
        }

        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var redireccion = VerificarAdministrador();
        if (redireccion != null) return redireccion;

        var chofer = await _choferService.ObtenerPorIdAsync(id);

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

            await _choferService.EditarAsync(
                vm.ChoferId,
                vm.Identificacion,
                vm.Nombre,
                vm.Apellidos);

            TempData["MensajeExito"] = "Chofer actualizado correctamente.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ViewBag.MensajeError = ex.Message;
            return View(vm);
        }
    }
}