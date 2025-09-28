using Microsoft.AspNetCore.Mvc;
using InmobiliariaMVC.Models;
using InmobiliariaMVC.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace InmobiliariaMVC.Controllers
{
    [Authorize]
    public class ContratoController : Controller
    {
        private readonly ContratoRepositorio repoContrato = new ContratoRepositorio();
        private readonly InmuebleRepositorio repoInmueble = new InmuebleRepositorio();
        private readonly InquilinoRepositorio repoInquilino = new InquilinoRepositorio();
        private readonly PagoRepositorio repoPago = new PagoRepositorio();
        private readonly MultaService multaService = new MultaService();

        [Authorize(Roles = "Admin,Empleado")]
        public IActionResult Index()
        {
            var lista = repoContrato.ObtenerTodos();
            return View(lista);
        }

        [Authorize(Roles = "Admin,Empleado")]
        public IActionResult Create()
        {
            CargarListas();
            return View();
        }
        [Authorize(Roles = "Admin,Empleado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Contrato contrato)
        {
            if (!ModelState.IsValid)
            {
                CargarListas();
                return View(contrato);
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            repoContrato.Alta(contrato, userId);
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin,Empleado")]
        public IActionResult Edit(int id)
        {
            var contrato = repoContrato.ObtenerPorId(id);
            if (contrato == null) return NotFound();

            ViewBag.Inquilinos = new SelectList(repoInquilino.ObtenerTodos(), "id_inquilino", "nombre", contrato.id_inquilino);
            ViewBag.Inmuebles = new SelectList(repoInmueble.ObtenerTodos(), "id_inmueble", "direccion", contrato.id_inmueble);

            return View(contrato);
        }
        [Authorize(Roles = "Admin,Empleado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Contrato contrato)
        {
            if (id != contrato.id_contrato) return BadRequest();

            if (ModelState.IsValid)
            {
                repoContrato.Modificacion(contrato);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Inquilinos = new SelectList(repoInquilino.ObtenerTodos(), "id_inquilino", "nombre", contrato.id_inquilino);
            ViewBag.Inmuebles = new SelectList(repoInmueble.ObtenerTodos(), "id_inmueble", "direccion", contrato.id_inmueble);
            return View(contrato);
        }
        [Authorize(Roles = "Admin,Empleado")]
        public IActionResult Details(int id)
        {
            var contrato = repoContrato.ObtenerPorId(id);
            if (contrato == null) return NotFound();
            return View(contrato);
        }
        [Authorize(Roles = "Admin,Empleado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Terminar(int id)
        {
            var contrato = repoContrato.ObtenerPorId(id);
            if (contrato == null) return NotFound();

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            repoContrato.TerminarContrato(id, userId);

            TempData["Success"] = $"Contrato #{id} finalizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Empleado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TerminarConMulta(int id)
        {
            var contrato = repoContrato.ObtenerPorId(id);
            if (contrato == null) return NotFound();

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

            var montoMulta = multaService.CalcularMulta(contrato);

            var pago = new Pago
            {
                id_contrato = contrato.id_contrato,
                fecha = DateTime.Now,
                importe = montoMulta,
                detalle = "Multa por terminación anticipada",
                nro_pago = (repoPago.ObtenerPorContrato(contrato.id_contrato)?.Count ?? 0) + 1
            };
            repoPago.Alta(pago, userId);

            repoContrato.TerminarContrato(id, userId);

            TempData["Success"] = $"Contrato #{contrato.id_contrato} finalizado con multa de ${montoMulta}";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Empleado")]
        [HttpGet]
        public IActionResult CalcularMulta(int contratoId)
        {
            var contrato = repoContrato.ObtenerPorId(contratoId);
            if (contrato == null) return NotFound();

            var monto = multaService.CalcularMulta(contrato);
            return Json(new { monto });
        }

        private void CargarListas()
        {
            ViewBag.Inquilinos = new SelectList(repoInquilino.ObtenerTodos(), "id_inquilino", "nombre");
            ViewBag.Inmuebles = new SelectList(repoInmueble.ObtenerTodos(), "id_inmueble", "direccion");
        }

        [Authorize(Roles = "Admin,Empleado")]
        public IActionResult Delete(int id)
        {
            var contrato = repoContrato.ObtenerPorId(id);
            if (contrato == null) return NotFound();
            return View(contrato);
        }
        [Authorize(Roles = "Admin,Empleado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            repoContrato.Baja(id);
            return RedirectToAction(nameof(Index));
        }
        // GET: Contrato/Renovar/5
        [Authorize(Roles = "Admin,Empleado")]
        public IActionResult Renovar(int id)
        {
            var contrato = repoContrato.ObtenerPorId(id);
            if (contrato == null) return NotFound();

            // Cargo la vista con los datos actuales, pero listo para editar fechas/nuevo monto
            return View(contrato);
        }
        [Authorize(Roles = "Admin,Empleado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Renovar(int id, Contrato contrato)
        {
            if (id != contrato.id_contrato) return BadRequest();

            if (ModelState.IsValid)
            {
                // Opción 1: Modificar el mismo contrato (actualizando fechas/monto).
                repoContrato.Modificacion(contrato);

                // Opción 2: Generar un contrato nuevo como "renovación"
                // repoContrato.Alta(contrato, userId);

                TempData["Success"] = $"Contrato #{id} renovado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(contrato);
        }
        // ------------------- INFORMES / FILTROS -------------------

        // 1) Contratos vigentes por fecha
        [Authorize(Roles = "Admin,Empleado")]
        [HttpGet]
        public IActionResult Vigentes(DateTime desde, DateTime hasta)
        {
            var contratos = repoContrato.ObtenerTodos()
                .Where(c => c.fecha_inicio >= desde && c.fecha_fin <= hasta && c.estado)
                .ToList();

            return View("Index", contratos); // Reutilizo la misma vista Index
        }

        // 2) Contratos por inmueble
        [Authorize(Roles = "Admin,Empleado")]
        [HttpGet]
        public IActionResult PorInmueble(int inmuebleId)
        {
            var contratos = repoContrato.ObtenerTodos()
                .Where(c => c.id_inmueble == inmuebleId)
                .ToList();

            return View("Index", contratos);
        }
        // 3) Contratos que vencen en X días
        [Authorize(Roles = "Admin,Empleado")]
        [HttpGet]
        public IActionResult PorVencimiento(int dias)
        {
            var fechaLimite = DateTime.Now.AddDays(dias);
            var contratos = repoContrato.ObtenerTodos()
                .Where(c => c.fecha_fin <= fechaLimite && c.estado)
                .ToList();

            return View("Index", contratos);
        }

    }
}
