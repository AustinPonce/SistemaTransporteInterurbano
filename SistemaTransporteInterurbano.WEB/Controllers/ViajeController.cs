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
    private readonly IPasajeroService _pasajeroService;

    public ViajeController(
        IViajeService viajeService,
        IRutaService rutaService,
        IUnidadService unidadService,
        IChoferService choferService,
        IPasajeroService pasajeroService,
        INotificacionCorreoService correoService)
    {
        _viajeService = viajeService;
        _rutaService = rutaService;
        _unidadService = unidadService;
        _choferService = choferService;
        _correoService = correoService;
        _pasajeroService = pasajeroService;
    }

    private IActionResult? VerificarAcceso()
    {
        var rol = HttpContext.Session.GetString("Rol");
        if (rol != Roles.Administrador && rol != Roles.Chofer)
            return RedirectToAction("IniciarSesion", "Autenticacion");
        return null;
    }

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

            if (!vm.RutaId.HasValue || !vm.UnidadId.HasValue || !vm.ChoferId.HasValue || !vm.FechaSalida.HasValue || !vm.FechaLlegadaEstimada.HasValue)
            {
                if (!vm.RutaId.HasValue) ModelState.AddModelError(nameof(vm.RutaId), "La ruta es requerida.");
                if (!vm.UnidadId.HasValue) ModelState.AddModelError(nameof(vm.UnidadId), "La unidad es requerida.");
                if (!vm.ChoferId.HasValue) ModelState.AddModelError(nameof(vm.ChoferId), "El chofer es requerido.");
                if (!vm.FechaSalida.HasValue) ModelState.AddModelError(nameof(vm.FechaSalida), "La fecha y hora de salida es requerida.");
                if (!vm.FechaLlegadaEstimada.HasValue) ModelState.AddModelError(nameof(vm.FechaLlegadaEstimada), "La fecha y hora estimada de llegada es requerida.");

                await CargarCatalogosAsync();
                return View(vm);
            }

            await _viajeService.AgregarAsync(
                vm.RutaId.Value,
                vm.UnidadId.Value,
                vm.ChoferId.Value,
                vm.FechaSalida.Value,
                vm.FechaLlegadaEstimada.Value);

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

            if (!vm.RutaId.HasValue || !vm.UnidadId.HasValue || !vm.ChoferId.HasValue || !vm.FechaSalida.HasValue || !vm.FechaLlegadaEstimada.HasValue)
            {
                if (!vm.RutaId.HasValue) ModelState.AddModelError(nameof(vm.RutaId), "La ruta es requerida.");
                if (!vm.UnidadId.HasValue) ModelState.AddModelError(nameof(vm.UnidadId), "La unidad es requerida.");
                if (!vm.ChoferId.HasValue) ModelState.AddModelError(nameof(vm.ChoferId), "El chofer es requerido.");
                if (!vm.FechaSalida.HasValue) ModelState.AddModelError(nameof(vm.FechaSalida), "La fecha y hora de salida es requerida.");
                if (!vm.FechaLlegadaEstimada.HasValue) ModelState.AddModelError(nameof(vm.FechaLlegadaEstimada), "La fecha y hora estimada de llegada es requerida.");

                await CargarCatalogosAsync();
                return View(vm);
            }

            await _viajeService.EditarAsync(
                vm.ViajeId,
                vm.RutaId.Value,
                vm.UnidadId.Value,
                vm.ChoferId.Value,
                vm.FechaSalida.Value,
                vm.FechaLlegadaEstimada.Value);

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

    [HttpGet]
    public async Task<IActionResult> Cancelados()
    {
        var redireccion = VerificarAcceso();
        if (redireccion != null) return redireccion;

        var viajes = await _viajeService.ObtenerCanceladosAsync();
        return View(viajes);
    }

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
    [HttpGet]
    public async Task<IActionResult> EnCurso()
    {
        var redireccion = VerificarAcceso();
        if (redireccion != null) return redireccion;

        var viajes = await _viajeService.ObtenerActivosAsync();
        return View(viajes);
    }

    [HttpGet]
    public async Task<IActionResult> Reservar(int id)
    {
        var redireccion = VerificarAcceso();
        if (redireccion != null) return redireccion;

        var viaje = await _viajeService.ObtenerPorIdAsync(id);
        if (viaje == null || viaje.Estado != EstadoViaje.EnCurso)
            return RedirectToAction("EnCurso");

        var pasajeros = await _pasajeroService.ObtenerTodosAsync();

        var asientosOcupados = viaje.Reservas
            .Select(r => r.NumeroAsiento)
            .ToList();

        ViewBag.Viaje = viaje;
        ViewBag.Pasajeros = pasajeros.Select(p => new SelectListItem
        {
            Value = p.PasajeroId.ToString(),
            Text = $"{p.Nombre} {p.Apellidos} - {p.Identificacion}"
        }).ToList();

        ViewBag.Capacidad = viaje.Unidad.CapacidadPasajeros;
        ViewBag.AsientosOcupados = asientosOcupados;

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reservar(int viajeId, int pasajeroId, int numeroAsiento)
    {
        var redireccion = VerificarAcceso();
        if (redireccion != null) return redireccion;

        try
        {
            await _viajeService.ReservarAsientoAsync(viajeId, pasajeroId, numeroAsiento);
            TempData["MensajeExito"] = "Reserva registrada correctamente.";
            return RedirectToAction("Pasajeros", new { id = viajeId });
        }
        catch (Exception ex)
        {
            TempData["MensajeError"] = ex.Message;
            return RedirectToAction("Reservar", new { id = viajeId });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Pasajeros(int id)
    {
        var redireccion = VerificarAcceso();
        if (redireccion != null) return redireccion;

        var viaje = await _viajeService.ObtenerPorIdAsync(id);
        if (viaje == null)
            return RedirectToAction("EnCurso");

        var reservas = await _viajeService.ObtenerPasajerosAsync(id);
        var totales = await _viajeService.ObtenerTotalesAsync(id);

        ViewBag.Viaje = viaje;
        ViewBag.Pasajeros = totales.pasajeros;
        ViewBag.Disponibles = totales.disponibles;
        ViewBag.Total = totales.total;

        return View(reservas);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelarReserva(int reservaId, int viajeId)
    {
        var redireccion = VerificarAcceso();
        if (redireccion != null) return redireccion;

        try
        {
            await _viajeService.CancelarReservaAsync(reservaId);
            TempData["MensajeExito"] = "Reserva cancelada correctamente.";
        }
        catch (Exception ex)
        {
            TempData["MensajeError"] = ex.Message;
        }

        return RedirectToAction("Pasajeros", new { id = viajeId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Finalizar(int id)
    {
        var redireccion = VerificarAcceso();
        if (redireccion != null) return redireccion;

        try
        {
            await _viajeService.FinalizarViajeAsync(id);
            TempData["MensajeExito"] = "Viaje finalizado correctamente.";
        }
        catch (Exception ex)
        {
            TempData["MensajeError"] = ex.Message;
        }

        return RedirectToAction("EnCurso");
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmarFinalizar(int id)
    {
        var redireccion = VerificarAcceso();
        if (redireccion != null) return redireccion;

        var viaje = await _viajeService.ObtenerDetalleAsync(id);
        if (viaje == null || viaje.Estado != EstadoViaje.EnCurso)
            return RedirectToAction("EnCurso");

        var totales = await _viajeService.ObtenerTotalesAsync(id);

        var vm = new SistemaTransporteInterurbano.WEB.Models.ViewModels.FinalizarViajeViewModel
        {
            ViajeId = viaje.ViajeId,
            RutaNombre = viaje.Ruta?.Nombre ?? "—",
            PlacaUnidad = viaje.Unidad?.Placa ?? "—",
            FechaSalida = viaje.FechaSalida,
            Pasajeros = totales.pasajeros,
            Disponibles = totales.disponibles,
            TotalRecaudacion = totales.total
        };

        return View(vm);
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
