using Microsoft.AspNetCore.Mvc;
using SistemaTransporteInterurbano.BL.Interfaces;
using SistemaTransporteInterurbano.Models;

namespace SistemaTransporteInterurbano.WEB.Controllers
{
    public class MisViajesController : Controller
    {
        private readonly IViajeService _viajeService;
        private readonly IPasajeroService _pasajeroService;

        public MisViajesController(
            IViajeService viajeService,
            IPasajeroService pasajeroService)
        {
            _viajeService = viajeService;
            _pasajeroService = pasajeroService;
        }

        private IActionResult? VerificarAcceso()
        {
            var rol = HttpContext.Session.GetString("Rol");

            if (rol != Roles.Pasajero)
                return RedirectToAction("IniciarSesion", "Autenticacion");

            return null;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var redireccion = VerificarAcceso();
            if (redireccion != null) return redireccion;

            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");

            if (usuarioId == null)
                return RedirectToAction("IniciarSesion", "Autenticacion");

            var pasajero = await _pasajeroService.ObtenerPorUsuarioIdAsync(usuarioId.Value);

            if (pasajero == null)
            {
                TempData["MensajeError"] = "No se encontró el pasajero asociado al usuario actual.";
                return View(new List<SistemaTransporteInterurbano.Models.Entities.Reserva>());
            }

            var reservas = await _viajeService.ObtenerReservasPasajeroAsync(pasajero.PasajeroId);

            return View(reservas);
        }

        [HttpGet]
        public async Task<IActionResult> Detalle(int id)
        {
            var redireccion = VerificarAcceso();
            if (redireccion != null) return redireccion;

            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");

            if (usuarioId == null)
                return RedirectToAction("IniciarSesion", "Autenticacion");

            var pasajero = await _pasajeroService.ObtenerPorUsuarioIdAsync(usuarioId.Value);

            if (pasajero == null)
                return RedirectToAction("Index");

            var viaje = await _viajeService.ObtenerDetalleAsync(id);

            if (viaje == null)
                return RedirectToAction("Index");

            var reserva = viaje.Reservas
                .FirstOrDefault(r => r.PasajeroId == pasajero.PasajeroId);

            if (reserva == null)
                return RedirectToAction("Index");

            ViewBag.Reserva = reserva;

            return View(viaje);
        }
    }
}