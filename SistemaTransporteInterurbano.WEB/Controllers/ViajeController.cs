using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaTransporteInterurbano.BL.Interfaces;
using SistemaTransporteInterurbano.Models;
using SistemaTransporteInterurbano.Models.Entities;
using SistemaTransporteInterurbano.WEB.Models.ViewModels;

namespace SistemaTransporteInterurbano.WEB.Controllers;

public class ViajeController : Controller
{
    private readonly IViajeService _viajeService;
    private readonly IRutaService _rutaService;
    private readonly IUnidadService _unidadService;
    private readonly IChoferService _choferService;
    private readonly INotificacionCorreoService _correoService;

    public ViajeController(
        IViajeService viajeService,
        IRutaService rutaService,
        IUnidadService unidadService,
        IChoferService choferService,
        INotificacionCorreoService correoService)
    {
        _viajeService = viajeService;
        _rutaService = rutaService;
        _unidadService = unidadService;
        _choferService = choferService;
        _correoService = correoService;
    }

    private IActionResult? VerificarAcceso()
    {
        var rol = HttpContext.Session.GetString("Rol");
        if (rol != Roles.Administrador && rol != Roles.Chofer)
            return RedirectToAction("IniciarSesion", "Autenticacion");
        return null;
    }

    // Req #24 — Listar con filtro por ruta o fecha
    [HttpGet]
    public async Task<IActionResult> Index(string? filtroRuta, DateTime? filtroFecha)
    {
        var redireccion = VerificarAcceso();
        if (redireccion != null) return redireccion;

        var viajes = await _viajeService.ObtenerTodosAsync(filtroRuta, filtroFecha);
        ViewBag.FiltroRuta = filtroRuta;
        ViewBag.FiltroFecha = filtroFecha?.ToString("yyyy-MM-dd");
        return View(viajes);
    }

    // Req #25 — Agregar viaje
    [HttpGet]
    public async Task<IActionResult> Agregar()
    {
        var redireccion = VerificarAcceso();
        if (redireccion != null) return redireccion;

        await CargarCatalogosAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Agregar(AgregarViajeViewModel vm)
    {
        var redireccion = VerificarAcceso();
        if (redireccion != null) return redireccion;

        try
        {
            if (!ModelState.IsValid)
            {
                await CargarCatalogosAsync();
                return View(vm);
            }

            await _viajeService.AgregarAsync(
                vm.RutaId,
                vm.UnidadId,
                vm.ChoferId,
                vm.FechaSalida,
                vm.FechaLlegadaEstimada);

            TempData["MensajeExito"] = "Viaje registrado correctamente.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ViewBag.MensajeError = ex.Message;
            await CargarCatalogosAsync();
            return View(vm);
        }
    }

    // Req #28-29 — Editar viaje (solo Programado)
    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var redireccion = VerificarAcceso();
        if (redireccion != null) return redireccion;

        var viaje = await _viajeService.ObtenerPorIdAsync(id);
        if (viaje == null)
            return RedirectToAction("Index");

        var vm = new EditarViajeViewModel
        {
            ViajeId = viaje.ViajeId,
            RutaId = viaje.RutaId,
            UnidadId = viaje.UnidadId,
            ChoferId = viaje.ChoferId,
            FechaSalida = viaje.FechaSalida,
            FechaLlegadaEstimada = viaje.FechaLlegadaEstimada
        };

        await CargarCatalogosAsync();
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Editar(EditarViajeViewModel vm)
    {
        var redireccion = VerificarAcceso();
        if (redireccion != null) return redireccion;

        try
        {
            if (!ModelState.IsValid)
            {
                await CargarCatalogosAsync();
                return View(vm);
            }

            await _viajeService.EditarAsync(
                vm.ViajeId,
                vm.RutaId,
                vm.UnidadId,
                vm.ChoferId,
                vm.FechaSalida,
                vm.FechaLlegadaEstimada);

            TempData["MensajeExito"] = "Viaje actualizado correctamente.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ViewBag.MensajeError = ex.Message;
            await CargarCatalogosAsync();
            return View(vm);
        }
    }

    // Req #30-31 — Cancelar viaje con motivo y notificación
    [HttpGet]
    public async Task<IActionResult> Cancelar(int id)
    {
        var redireccion = VerificarAcceso();
        if (redireccion != null) return redireccion;

        var viaje = await _viajeService.ObtenerPorIdAsync(id);
        if (viaje == null)
            return RedirectToAction("Index");

        var vm = new CancelarViajeViewModel
        {
            ViajeId = viaje.ViajeId,
            RutaNombre = viaje.Ruta?.Nombre ?? "—",
            FechaSalida = viaje.FechaSalida,
            PlacaUnidad = viaje.Unidad?.Placa ?? "—"
        };
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Cancelar(CancelarViajeViewModel vm)
    {
        var redireccion = VerificarAcceso();
        if (redireccion != null) return redireccion;

        try
        {
            if (!ModelState.IsValid)
                return View(vm);

            await _viajeService.CancelarAsync(vm.ViajeId, vm.Motivo, _correoService);

            TempData["MensajeExito"] = "Viaje cancelado correctamente. Se notificó a los pasajeros con reserva.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ViewBag.MensajeError = ex.Message;
            return View(vm);
        }
    }

    // Req #32 — Iniciar viaje
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Iniciar(int id)
    {
        var redireccion = VerificarAcceso();
        if (redireccion != null) return redireccion;

        try
        {
            await _viajeService.IniciarAsync(id);
            TempData["MensajeExito"] = "Viaje iniciado. Estado cambiado a En Curso.";
        }
        catch (Exception ex)
        {
            TempData["MensajeError"] = ex.Message;
        }

        return RedirectToAction("Index");
    }

    // ── MÓDULO 8 — Viajes Cancelados ─────────────────────────────────

    // Req #43 — Listar viajes cancelados
    [HttpGet]
    public async Task<IActionResult> Cancelados()
    {
        var redireccion = VerificarAcceso();
        if (redireccion != null) return redireccion;

        var viajes = await _viajeService.ObtenerCanceladosAsync();
        return View(viajes);
    }

    // Req #44 — Detalle del viaje cancelado (incluye motivo)
    [HttpGet]
    public async Task<IActionResult> DetalleCancelado(int id)
    {
        var redireccion = VerificarAcceso();
        if (redireccion != null) return redireccion;

        var viaje = await _viajeService.ObtenerPorIdAsync(id);
        if (viaje == null || viaje.Estado != EstadoViaje.Cancelado)
            return RedirectToAction("Cancelados");

        return View(viaje);
    }

    private async Task CargarCatalogosAsync()
    {
        var rutas = await _rutaService.ObtenerTodasAsync();
        var unidades = await _unidadService.ObtenerTodasAsync();
        var choferes = await _choferService.ObtenerTodosAsync();

        ViewBag.Rutas = rutas.Select(r => new SelectListItem
        {
            Value = r.RutaId.ToString(),
            Text = $"{r.Nombre} ({r.Origen} → {r.Destino})"
        }).ToList();

        ViewBag.Unidades = unidades.Select(u => new SelectListItem
        {
            Value = u.UnidadId.ToString(),
            Text = $"{u.Placa} — {u.Modelo} (Cap. {u.CapacidadPasajeros})"
        }).ToList();

        ViewBag.Choferes = choferes.Select(c => new SelectListItem
        {
            Value = c.ChoferId.ToString(),
            Text = $"{c.Nombre} {c.Apellidos}"
        }).ToList();
    }
}
