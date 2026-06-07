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

        ViewBag.Modelos = new List<string>
        {
            "Mercedes-Benz O500",
            "Mercedes-Benz O371",
            "Marcopolo Paradiso",
            "Scania K310",
            "Volvo 9700",
            "Toyota Coaster",
            "Hino AK",
            "Isuzu FRR",
            "Volkswagen 15.210",
            "Agrale MA 10.0",
            "Mercedes-Benz Sprinter",
            "Iveco Daily",
            "Ford Transit",
            "Chevrolet NKR",
            "Dongfeng DFA"
        };

        ViewBag.UnidadesPredeterminadas = new List<Dictionary<string, string>>
        {
            new() { { "Placa", "ABC001" }, { "Modelo", "Mercedes-Benz O500" }, { "Anio", "2022" }, { "Capacidad", "45" } },
            new() { { "Placa", "ABC002" }, { "Modelo", "Marcopolo Paradiso" }, { "Anio", "2023" }, { "Capacidad", "50" } },
            new() { { "Placa", "ABC003" }, { "Modelo", "Toyota Coaster" }, { "Anio", "2021" }, { "Capacidad", "30" } },
            new() { { "Placa", "ABC004" }, { "Modelo", "Scania K310" }, { "Anio", "2023" }, { "Capacidad", "55" } },
            new() { { "Placa", "ABC005" }, { "Modelo", "Hino AK" }, { "Anio", "2020" }, { "Capacidad", "40" } }
        };

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
            {
                ViewBag.Modelos = new List<string>
                {
                    "Mercedes-Benz O500",
                    "Mercedes-Benz O371",
                    "Marcopolo Paradiso",
                    "Scania K310",
                    "Volvo 9700",
                    "Toyota Coaster",
                    "Hino AK",
                    "Isuzu FRR",
                    "Volkswagen 15.210",
                    "Agrale MA 10.0",
                    "Mercedes-Benz Sprinter",
                    "Iveco Daily",
                    "Ford Transit",
                    "Chevrolet NKR",
                    "Dongfeng DFA"
                };

                return View(vm);
            }

            if (!vm.AnioFabricacion.HasValue)
            {
                ModelState.AddModelError(nameof(vm.AnioFabricacion), "El año de fabricación es requerido.");
                return View(vm);
            }

            if (!vm.CapacidadPasajeros.HasValue)
            {
                ModelState.AddModelError(nameof(vm.CapacidadPasajeros), "La capacidad es requerida.");
                return View(vm);
            }

            await _unidadService.AgregarAsync(
                vm.Placa,
                vm.Modelo,
                vm.AnioFabricacion.Value,
                vm.CapacidadPasajeros.Value);

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

        ViewBag.Modelos = new List<string>
        {
            "Mercedes-Benz O500",
            "Mercedes-Benz O371",
            "Marcopolo Paradiso",
            "Scania K310",
            "Volvo 9700",
            "Toyota Coaster",
            "Hino AK",
            "Isuzu FRR",
            "Volkswagen 15.210",
            "Agrale MA 10.0",
            "Mercedes-Benz Sprinter",
            "Iveco Daily",
            "Ford Transit",
            "Chevrolet NKR",
            "Dongfeng DFA"
        };

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
            {
                ViewBag.Modelos = new List<string>
                {
                    "Mercedes-Benz O500",
                    "Mercedes-Benz O371",
                    "Marcopolo Paradiso",
                    "Scania K310",
                    "Volvo 9700",
                    "Toyota Coaster",
                    "Hino AK",
                    "Isuzu FRR",
                    "Volkswagen 15.210",
                    "Agrale MA 10.0",
                    "Mercedes-Benz Sprinter",
                    "Iveco Daily",
                    "Ford Transit",
                    "Chevrolet NKR",
                    "Dongfeng DFA"
                };

                return View(vm);
            }

            if (!vm.AnioFabricacion.HasValue)
            {
                ModelState.AddModelError(nameof(vm.AnioFabricacion), "El año de fabricación es requerido.");
                return View(vm);
            }

            if (!vm.CapacidadPasajeros.HasValue)
            {
                ModelState.AddModelError(nameof(vm.CapacidadPasajeros), "La capacidad es requerida.");
                return View(vm);
            }

            await _unidadService.EditarAsync(
                vm.UnidadId,
                vm.Placa,
                vm.Modelo,
                vm.AnioFabricacion.Value,
                vm.CapacidadPasajeros.Value);

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