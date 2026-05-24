using Microsoft.AspNetCore.Mvc;
using SistemaTransporteInterurbano.BL.Interfaces;
using SistemaTransporteInterurbano.Models;
using SistemaTransporteInterurbano.WEB.Models.ViewModels;

namespace SistemaTransporteInterurbano.WEB.Controllers;

public class UnidadController : Controller
{
    private readonly IUnidadService _unidadService;

    public UnidadController(IUnidadService unidadService)
    {
        _unidadService = unidadService;
    }

    private IActionResult? VerificarAcceso()
    {
        var rol = HttpContext.Session.GetString("Rol");
        if (rol != Roles.Administrador && rol != Roles.Chofer)
            return RedirectToAction("IniciarSesion", "Autenticacion");
        return null;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var redireccion = VerificarAcceso();
        if (redireccion != null) return redireccion;

        var unidades = await _unidadService.ObtenerTodasAsync();
        return View(unidades);
    }

    [HttpGet]
    public IActionResult Agregar()
    {
        var redireccion = VerificarAcceso();
        if (redireccion != null) return redireccion;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Agregar(AgregarUnidadViewModel vm)
    {
        var redireccion = VerificarAcceso();
        if (redireccion != null) return redireccion;

        try
        {
            if (!ModelState.IsValid)
                return View(vm);

            await _unidadService.AgregarAsync(
                vm.Placa,
                vm.Modelo,
                vm.AnioFabricacion,
                vm.CapacidadPasajeros);

            TempData["MensajeExito"] = "Unidad registrada correctamente.";
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
        var redireccion = VerificarAcceso();
        if (redireccion != null) return redireccion;

        var unidad = await _unidadService.ObtenerPorIdAsync(id);

        if (unidad == null)
            return RedirectToAction("Index");

        var vm = new EditarUnidadViewModel
        {
            UnidadId = unidad.UnidadId,
            Placa = unidad.Placa,
            Modelo = unidad.Modelo,
            AnioFabricacion = unidad.AnioFabricacion,
            CapacidadPasajeros = unidad.CapacidadPasajeros
        };

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Editar(EditarUnidadViewModel vm)
    {
        var redireccion = VerificarAcceso();
        if (redireccion != null) return redireccion;

        try
        {
            if (!ModelState.IsValid)
                return View(vm);

            await _unidadService.EditarAsync(
                vm.UnidadId,
                vm.Placa,
                vm.Modelo,
                vm.AnioFabricacion,
                vm.CapacidadPasajeros);

            TempData["MensajeExito"] = "Unidad actualizada correctamente.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ViewBag.MensajeError = ex.Message;
            return View(vm);
        }
    }
}