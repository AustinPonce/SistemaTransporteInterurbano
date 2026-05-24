using Microsoft.AspNetCore.Mvc;
using SistemaTransporteInterurbano.Models;

namespace SistemaTransporteInterurbano.WEB.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        var rol = HttpContext.Session.GetString("Rol");

        if (rol == Roles.Administrador)
            return View("Index");

        if (rol == Roles.Chofer)
            return View("IndexChofer");

        return RedirectToAction("IniciarSesion", "Autenticacion");
    }
}

/*using Microsoft.AspNetCore.Mvc;
using SistemaTransporteInterurbano.BL.Helpers;

namespace SistemaTransporteInterurbano.WEB.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        var hash =
            HashHelper.GenerarHash(
                "TicoBus2025*");

        return Content(hash);
    }
}
*/
