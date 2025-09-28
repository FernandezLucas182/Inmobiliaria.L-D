using Microsoft.AspNetCore.Mvc;
using InmobiliariaMVC.Models;
using InmobiliariaMVC.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace InmobiliariaMVC.Controllers
{   
    [Authorize]
    public class PagosController : Controller
    {
        private readonly PagoRepositorio repoPago = new PagoRepositorio();
        private readonly ContratoRepositorio repoContrato = new ContratoRepositorio();
        private readonly MultaService multaService = new MultaService();

        // Listado de pagos
        [Authorize(Roles = "Admin,Empleado")]
        public IActionResult Index()
        {
            var lista = repoPago.ObtenerTodos();
            return View(lista);
        }

        // Detalles de un pago
        [Authorize(Roles = "Admin,Empleado")]
        public IActionResult Details(int id)
        {
            var pago = repoPago.ObtenerPorId(id);
            if (pago == null) return NotFound();

            ViewBag.IsAdmin = User.IsInRole("Admin");
            return View(pago);
        }

        // Crear pago
        [Authorize(Roles = "Admin,Empleado")]
        public IActionResult Create()
        {
            CargarContratos();
            return View(new Pago { fecha = DateTime.Today });
        }
        [Authorize(Roles = "Admin,Empleado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Pago pago)
        {
            if (ModelState.IsValid)
            {
                if (pago.id_contrato > 0)
                {
                    var pagosExistentes = repoPago.ObtenerPorContrato(pago.id_contrato);
                    pago.nro_pago = (pagosExistentes?.Count ?? 0) + 1;
                }

                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
                repoPago.Alta(pago, userId);

                return RedirectToAction(nameof(Index));
            }

            CargarContratos(pago.id_contrato);
            return View(pago);
        }

        // Editar pago
        [Authorize(Roles = "Admin,Empleado")]
        public IActionResult Edit(int id)
        {
            var pago = repoPago.ObtenerPorId(id);
            if (pago == null || !pago.estado) return NotFound();
            return View(pago);
        }

        [Authorize(Roles = "Admin,Empleado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Pago pago)
        {
            if (id != pago.id_pago) return BadRequest();

            if (ModelState.IsValid)
            {
                repoPago.Modificacion(pago);
                return RedirectToAction(nameof(Index));
            }

            return View(pago);
        }
        [Authorize(Roles = "Admin,Empleado")]
        public IActionResult Cancelar(int id)
        {
            var pago = repoPago.ObtenerPorId(id);
            if (pago == null || !pago.estado) return NotFound();
            return View(pago);
        }
        [Authorize(Roles = "Admin,Empleado")]
        [HttpPost, ActionName("Cancelar")]
        [ValidateAntiForgeryToken]
        public IActionResult CancelarConfirmed(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            repoPago.Baja(id, userId);
            return RedirectToAction(nameof(Index));
        }

        // Registrar multa (GET)
        [Authorize(Roles = "Admin,Empleado")]
        public IActionResult RegistrarMulta(int contratoId)
        {
            var contrato = repoContrato.ObtenerPorId(contratoId);
            if (contrato == null) return NotFound();

            if (!contrato.estado)
            {
                TempData["Error"] = "El contrato ya está cerrado.";
                return RedirectToAction("PorContrato", new { idContrato = contratoId });
            }

            decimal multa = multaService.CalcularMulta(contrato);

            var pago = new Pago
            {
                id_contrato = contrato.id_contrato,
                fecha = DateTime.Now,
                importe = multa,
                detalle = "Multa por terminación anticipada",
                nro_pago = (repoPago.ObtenerPorContrato(contrato.id_contrato)?.Count ?? 0) + 1
            };

            ViewBag.Contrato = contrato;
            return View(pago);
        }

        // Registrar multa (POST)
        [Authorize(Roles = "Admin,Empleado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RegistrarMulta(Pago pago)
        {
            if (ModelState.IsValid)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
                repoPago.Alta(pago, userId);
                repoContrato.TerminarContrato(pago.id_contrato, userId);

                return RedirectToAction("PorContrato", new { idContrato = pago.id_contrato });
            }

            var contrato = repoContrato.ObtenerPorId(pago.id_contrato);
            ViewBag.Contrato = contrato;
            return View(pago);
        }

        // Calcular multa para finalización anticipada (JSON)   
        [Authorize(Roles = "Admin,Empleado")]
        [HttpGet]
        public IActionResult CalcularMulta(int contratoId)
        {
            var contrato = repoContrato.ObtenerPorId(contratoId);
            if (contrato == null) return NotFound();

            var monto = multaService.CalcularMulta(contrato);
            return Json(new { monto });
        }

        // Listado de pagos por contrato
        [Authorize(Roles = "Admin,Empleado")]
        public IActionResult PorContrato(int idContrato)
        {
            var lista = repoPago.ObtenerPorContrato(idContrato);
            if (lista == null || lista.Count == 0)
            {
                TempData["Mensaje"] = "No se encontraron pagos para este contrato.";
                return RedirectToAction("BuscarPorContrato");
            }

            ViewBag.Inquilino = $"{lista.First().Contrato?.Inquilino?.nombre} {lista.First().Contrato?.Inquilino?.apellido}";
            ViewBag.ContratoId = idContrato;

            return View(lista);
        }

        // Buscar pagos por contrato
        [Authorize(Roles = "Admin,Empleado")]
        public IActionResult BuscarPorContrato()
        {
            var contratos = new ContratoRepositorio().ObtenerTodos();
            ViewBag.Contratos = contratos;
            return View();
        }

        // Crear pago desde contrato
        [Authorize(Roles = "Admin,Empleado")]
        [HttpGet]
        public IActionResult CreatePorContrato(int idContrato)
        {
            var contrato = new ContratoRepositorio().ObtenerPorId(idContrato);
            if (contrato == null) return NotFound();

            var pagosExistentes = repoPago.ObtenerPorContrato(idContrato);
            int nroPagoSiguiente = (pagosExistentes?.Count ?? 0) + 1;

            var pago = new Pago
            {
                id_contrato = idContrato,
                fecha = DateTime.Today,
                nro_pago = nroPagoSiguiente
            };

            return View("Create", pago);
        }

        // Obtener siguiente número de pago (JSON)
        [Authorize(Roles = "Admin,Empleado")]
        [HttpGet]
        public JsonResult ObtenerNroPago(int idContrato)
        {
            var pagosExistentes = repoPago.ObtenerPorContrato(idContrato);
            int siguientePago = (pagosExistentes?.Count ?? 0) + 1;
            return Json(new { nro_pago = siguientePago });
        }

        // Método privado para cargar contratos en los dropdowns
        private void CargarContratos(int contratoSeleccionado = 0)
        {
            var contratos = repoContrato.ObtenerContratosConDetalle();
            ViewBag.Contratos = new SelectList(
                contratos.Select(c => new
                {
                    id_contrato = c.id_contrato,
                    Display = $"{c.Inquilino?.apellido}, {c.Inquilino?.nombre} - {c.Inmueble?.direccion}"
                }),
                "id_contrato",
                "Display",
                contratoSeleccionado
            );
        }
    }
}
