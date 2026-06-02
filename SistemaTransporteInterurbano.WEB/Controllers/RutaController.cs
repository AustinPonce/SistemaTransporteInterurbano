using Microsoft.AspNetCore.Mvc;
using SistemaTransporteInterurbano.BL.Interfaces;
using SistemaTransporteInterurbano.Models;
using SistemaTransporteInterurbano.WEB.Models.ViewModels;

namespace SistemaTransporteInterurbano.WEB.Controllers;

public class RutaController : Controller
{
    private readonly IRutaService _rutaService;

    public RutaController(IRutaService rutaService)
    {
        _rutaService = rutaService;
    }

    private IActionResult? VerificarAcceso()
    {
        var rol = HttpContext.Session.GetString("Rol");

        if (rol != Roles.Administrador && rol != Roles.Chofer)
            return RedirectToAction("IniciarSesion", "Autenticacion");

        return null;
    }

    private void CargarDestinos()
    {
        ViewBag.Destinos = new List<string>
        {
            "San José", "Alajuela", "Cartago", "Heredia", "Puntarenas",
            "Liberia", "Limón", "Pérez Zeledón", "Turrialba", "Grecia",
            "San Ramón", "Quesada", "Nicoya", "Santa Cruz", "Cañas",
            "Ciudad Neily", "Golfito", "Guápiles", "Siquirres", "Parrita"
        };
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? filtro)
    {
        var redireccion = VerificarAcceso();
        if (redireccion != null) return redireccion;

        var rutas = await _rutaService.ObtenerTodasAsync(filtro);

        ViewBag.Filtro = filtro;

        return View(rutas);
    }

    [HttpGet]
    public IActionResult Agregar()
    {
        var redireccion = VerificarAcceso();
        if (redireccion != null) return redireccion;

        CargarDestinos();

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
            {
                CargarDestinos();
                return View(vm);
            }

            if (!TimeSpan.TryParseExact(vm.DuracionEstimada, @"hh\:mm", null, out var duracion))
            {
                ViewBag.MensajeError = "El formato de duración es inválido. Use hh:mm.";
                CargarDestinos();
                return View(vm);
            }

            await _rutaService.AgregarAsync(
                vm.Nombre,
                vm.Origen,
                vm.Destino,
                duracion,
                vm.PrecioBase);

            TempData["MensajeExito"] = "Ruta registrada correctamente.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ViewBag.MensajeError = ex.Message;
            CargarDestinos();
            return View(vm);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var redireccion = VerificarAcceso();
        if (redireccion != null) return redireccion;

        var ruta = await _rutaService.ObtenerPorIdAsync(id);

        if (ruta == null)
            return RedirectToAction("Index");

        CargarDestinos();

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
            {
                CargarDestinos();
                return View(vm);
            }

            if (!TimeSpan.TryParseExact(vm.DuracionEstimada, @"hh\:mm", null, out var duracion))
            {
                ViewBag.MensajeError = "El formato de duración es inválido. Use hh:mm.";
                CargarDestinos();
                return View(vm);
            }

            await _rutaService.EditarAsync(
                vm.RutaId,
                vm.Nombre,
                vm.Origen,
                vm.Destino,
                duracion,
                vm.PrecioBase);

            TempData["MensajeExito"] = "Ruta actualizada correctamente.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ViewBag.MensajeError = ex.Message;
            CargarDestinos();
            return View(vm);
        }
    }
}