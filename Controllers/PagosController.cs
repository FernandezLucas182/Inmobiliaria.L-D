using Microsoft.AspNetCore.Mvc;
using InmobiliariaMVC.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;

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

            ViewBag.IsAdmin = User.IsInRole("Admin");
            return View(pago);
        }
        // GET: Pagos/Create
        public IActionResult Create()
        {
            var contratos = repoContrato.ObtenerContratosConDetalle();
            ViewBag.Contratos = new SelectList(
                contratos.Select(c => new
                {
                    id_contrato = c.id_contrato,
                    Display = $"{c.Inquilino?.apellido}, {c.Inquilino?.nombre} - {c.Inmueble?.direccion}"
                }),
                "id_contrato",
                "Display"
            );

            var pago = new Pago
            {
                fecha = DateTime.Today
            };

            return View(pago);
        }


        // POST: Pagos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Pago pago)
        {
            if (ModelState.IsValid)
            {
                var repo = new PagoRepositorio();

                //  Genera el número de pago automáticamente
                if (pago.id_contrato > 0)
                {
                    var pagosExistentes = repo.ObtenerPorContrato(pago.id_contrato);
                    pago.nro_pago = (pagosExistentes?.Count ?? 0) + 1;
                }

               // Auditoría: guardo el usuario que creó el pago
               var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
                repo.Alta(pago, userId);

                return RedirectToAction(nameof(Index));
            }

            // Si falla la validación, reconstruyo el SelectList
            var contratos = repoContrato.ObtenerContratosConDetalle();
            ViewBag.Contratos = new SelectList(
                contratos.Select(c => new
                {
                    id_contrato = c.id_contrato,
                    Display = $"{c.Inquilino?.apellido}, {c.Inquilino?.nombre} - {c.Inmueble?.direccion}"
                }),
                "id_contrato",
                "Display",
                pago.id_contrato
            );

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

        // Registrar multa como pago
        repoPago.Alta(pago, userId);

        // Cerrar contrato (con fecha de terminación anticipada)
        repoContrato.TerminarContrato(pago.id_contrato, userId);

        return RedirectToAction("PorContrato", new { idContrato = pago.id_contrato });
    }

    var contrato = repoContrato.ObtenerPorId(pago.id_contrato);
    ViewBag.Contrato = contrato;
    return View(pago);
}


        // Método privado para calcular multa
        private decimal CalcularMulta(Contrato contrato)
{
    int duracionTotal = (contrato.fecha_fin - contrato.fecha_inicio).Days;
    int diasCumplidos = (DateTime.Now - contrato.fecha_inicio).Days;

    int mesesMulta = diasCumplidos < duracionTotal / 2 ? 2 : 1;
    return contrato.monto * mesesMulta;
}

        // Buscar contrato antes de listar pagos
       public IActionResult BuscarPorContrato()
{
    var contratoRepo = new ContratoRepositorio();
    var contratos = contratoRepo.ObtenerTodos(); // asegúrate de que traiga Inquilino e Inmueble
    
    ViewBag.Contratos = contratos;

    return View();
}

        // Listar pagos por contrato
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

        // Crear pago directamente en un contrato
        [HttpGet]
        public IActionResult CreatePorContrato(int idContrato)
        {
            var repo = new PagoRepositorio();
            var contrato = new ContratoRepositorio().ObtenerPorId(idContrato);

            if (contrato == null)
            {
                return NotFound();
            }

            // 👉 Buscar la cantidad de pagos ya realizados para este contrato
            var pagosExistentes = repo.ObtenerPorContrato(idContrato);
            int nroPagoSiguiente = (pagosExistentes?.Count ?? 0) + 1;

            var pago = new Pago
            {
                id_contrato = idContrato,
                fecha = DateTime.Today,
                nro_pago = nroPagoSiguiente
            };

            return View("Create", pago);
        }
        // GET: Pagos/ObtenerNroPago
        [HttpGet]
        public JsonResult ObtenerNroPago(int idContrato)
        {
            var repo = new PagoRepositorio();
            var pagosExistentes = repo.ObtenerPorContrato(idContrato);
            int siguientePago = (pagosExistentes?.Count ?? 0) + 1;
            return Json(new { nro_pago = siguientePago });
        }




    }
}
