using Microsoft.AspNetCore.Mvc;
using SistemaTransporteInterurbano.Models;
using SistemaTransporteInterurbano.WEB.Models.ViewModels;
using SistemaTransporteInterurbano.WEB.Services;

namespace SistemaTransporteInterurbano.WEB.Controllers;

public class RutaController : Controller
{
    private readonly ServicioClienteApi _api;

    public RutaController(ServicioClienteApi api)
    {
        _api = api;
    }

    private IActionResult? VerificarAcceso()
    {
        var rol = HttpContext.Session.GetString("Rol");
        if (rol != Roles.Administrador && rol != Roles.Chofer)
            return RedirectToAction("IniciarSesion", "Autenticacion");
        return null;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? filtro)
    {
        var redireccion = VerificarAcceso();
        if (redireccion != null) return redireccion;

        var rutas = await _api.ObtenerRutasAsync(filtro);
        ViewBag.Filtro = filtro;
        return View(rutas);
    }

    [HttpGet]
    public IActionResult Agregar()
    {
        var redireccion = VerificarAcceso();
        if (redireccion != null) return redireccion;

        ViewBag.Destinos = new List<string>
        {
            "San José", "Alajuela", "Cartago", "Heredia", "Puntarenas",
            "Liberia", "Limón", "Pérez Zeledón", "Turrialba", "Grecia",
            "San Ramón", "Quesada", "Nicoya", "Santa Cruz", "Cañas",
            "Ciudad Neily", "Golfito", "Guápiles", "Siquirres", "Parrita"
        };

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Agregar(AgregarRutaViewModel vm)
    {
        var redireccion = VerificarAcceso();
        if (redireccion != null) return redireccion;

        try
        {
            if (!ModelState.IsValid)
                return View(vm);

            if (!TimeSpan.TryParseExact(vm.DuracionEstimada, @"hh\:mm", null, out var duracion))
            {
                ViewBag.MensajeError = "El formato de duración es inválido. Use hh:mm.";
                return View(vm);
            }

            await _api.AgregarRutaAsync(vm.Nombre, vm.Origen, vm.Destino, duracion, vm.PrecioBase);

            TempData["MensajeExito"] = "Ruta registrada correctamente.";
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

        ViewBag.Destinos = new List<string>
        {
            "San José", "Alajuela", "Cartago", "Heredia", "Puntarenas",
            "Liberia", "Limón", "Pérez Zeledón", "Turrialba", "Grecia",
            "San Ramón", "Quesada", "Nicoya", "Santa Cruz", "Cañas",
            "Ciudad Neily", "Golfito", "Guápiles", "Siquirres", "Parrita"
        };

        var ruta = await _api.ObtenerRutaPorIdAsync(id);

        if (ruta == null)
            return RedirectToAction("Index");

        var vm = new EditarRutaViewModel
        {
            RutaId = ruta.RutaId,
            Nombre = ruta.Nombre,
            Origen = ruta.Origen,
            Destino = ruta.Destino,
            DuracionEstimada = ruta.DuracionEstimada.ToString(@"hh\:mm"),
            PrecioBase = ruta.PrecioBase
        };

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Editar(EditarRutaViewModel vm)
    {
        var redireccion = VerificarAcceso();
        if (redireccion != null) return redireccion;

        try
        {
            if (!ModelState.IsValid)
                return View(vm);

            if (!TimeSpan.TryParseExact(vm.DuracionEstimada, @"hh\:mm", null, out var duracion))
            {
                ViewBag.MensajeError = "El formato de duración es inválido. Use hh:mm.";
                return View(vm);
            }

            await _api.EditarRutaAsync(vm.RutaId, vm.Nombre, vm.Origen, vm.Destino, duracion, vm.PrecioBase);

            TempData["MensajeExito"] = "Ruta actualizada correctamente.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ViewBag.MensajeError = ex.Message;
            return View(vm);
        }
    }
}
