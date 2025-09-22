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
            if (pago == null) return NotFound();

            ViewBag.Contratos = repoContrato.ObtenerTodos();
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
                repoPago.Modificacion(pago);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Contratos = repoContrato.ObtenerTodos();
            return View(pago);
        }

        // GET: Pagos/Delete/5
        public IActionResult Delete(int id)
        {
            var pago = repoPago.ObtenerPorId(id);
            if (pago == null) return NotFound();
            return View(pago);
        }

        // POST: Pagos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            repoPago.Baja(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: Pagos/Cancelar/5
public IActionResult Cancelar(int id)
{
    var pago = repoPago.ObtenerPorId(id);
    if (pago == null) return NotFound();
    return View(pago);
}

// POST: Pagos/Cancelar/5
[HttpPost, ActionName("Cancelar")]
[ValidateAntiForgeryToken]
public IActionResult CancelarConfirmed(int id)
{
    var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
    repoPago.CancelarPago(id, userId);
    return RedirectToAction(nameof(Index));
}

    }
}
