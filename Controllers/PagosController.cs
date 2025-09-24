using Microsoft.AspNetCore.Mvc;
using InmobiliariaMVC.Models;
using System.Security.Claims;

namespace InmobiliariaMVC.Controllers
{
    public class PagosController : Controller
    {
        private readonly PagoRepositorio repoPago = new PagoRepositorio();
        private readonly ContratoRepositorio repoContrato = new ContratoRepositorio();

        // GET: Pagos
        public IActionResult Index()
        {
            var lista = repoPago.ObtenerTodos();
            return View(lista);
        }

        // GET: Pagos/Details/5
        public IActionResult Details(int id)
        {
            var pago = repoPago.ObtenerPorId(id);
            if (pago == null) return NotFound();

            ViewBag.IsAdmin = User.IsInRole("Administrador");
            return View(pago);
        }

        // GET: Pagos/Create
        public IActionResult Create()
        {
            ViewBag.Contratos = repoContrato.ObtenerTodos();
            return View();
        }

        // POST: Pagos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Pago pago)
        {
            if (ModelState.IsValid)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
                repoPago.Alta(pago, userId);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Contratos = repoContrato.ObtenerTodos();
            return View(pago);
        }

        // GET: Pagos/Edit/5
        public IActionResult Edit(int id)
        {
            var pago = repoPago.ObtenerPorId(id);
            if (pago == null || !pago.estado) return NotFound(); // solo se edita si está activo
            return View(pago);
        }

        // POST: Pagos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Pago pago)
        {
            if (id != pago.id_pago) return BadRequest();

            if (ModelState.IsValid)
            {
                repoPago.Modificacion(pago); // solo se modifica detalle
                return RedirectToAction(nameof(Index));
            }

            return View(pago);
        }

        // GET: Pagos/Cancelar/5
        public IActionResult Cancelar(int id)
        {
            var pago = repoPago.ObtenerPorId(id);
            if (pago == null || !pago.estado) return NotFound();
            return View(pago);
        }

        // POST: Pagos/Cancelar/5
        [HttpPost, ActionName("Cancelar")]
        [ValidateAntiForgeryToken]
        public IActionResult CancelarConfirmed(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            repoPago.Baja(id, userId);
            return RedirectToAction(nameof(Index));
        }

        // GET: Pagos/PorContrato/5
        public IActionResult PagosPorContrato(int id) // id = id_contrato
        {
            var todos = repoPago.ObtenerTodos();
            var pagosContrato = todos.Where(p => p.id_contrato == id).ToList();
            ViewBag.ContratoId = id;
            return View(pagosContrato);
        }

        // GET: Pagos/RegistrarMulta/5
        public IActionResult RegistrarMulta(int contratoId)
        {
            var contrato = repoContrato.ObtenerPorId(contratoId);
            if (contrato == null) return NotFound();

            // Solo permitir si el contrato aún está activo
            if (!contrato.estado)
            {
                TempData["Error"] = "El contrato ya está cerrado.";
                return RedirectToAction("PagosPorContrato", new { id = contratoId });
            }

            // Calcular multa
            decimal multa = CalcularMulta(contrato);

            // Crear objeto pago temporal
            var pago = new Pago
            {
                id_contrato = contrato.id_contrato,
                fecha = DateTime.Now,
                importe = multa,
                detalle = "Multa por terminación anticipada",
                nro_pago = 0 // opcional: 0 o siguiente nro de pago
            };

            ViewBag.Contrato = contrato;
            return View(pago);
        }

        // POST: Pagos/RegistrarMulta/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RegistrarMulta(Pago pago)
        {
            if (ModelState.IsValid)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
                repoPago.Alta(pago, userId);

                // Opcional: Terminar contrato automáticamente
                repoContrato.TerminarContrato(pago.id_contrato, userId);

                return RedirectToAction("PagosPorContrato", new { id = pago.id_contrato });
            }

            var contrato = repoContrato.ObtenerPorId(pago.id_contrato);
            ViewBag.Contrato = contrato;
            return View(pago);
        }

        // Método privado para calcular multa
        private decimal CalcularMulta(Contrato contrato)
        {
            // Ejemplo: 1 mes de alquiler como multa proporcional
            // Se puede cambiar la regla según tus requerimientos
            decimal montoMensual = contrato.monto;
            int diasRestantes = (contrato.fecha_fin - DateTime.Now).Days;
            if (diasRestantes < 0) diasRestantes = 0;

            decimal multa = montoMensual * diasRestantes / 30; // proporcional a días restantes
            return Math.Round(multa, 2);
        }

    }
}
