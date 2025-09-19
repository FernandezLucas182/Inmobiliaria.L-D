using Microsoft.AspNetCore.Mvc;
using InmobiliariaMVC.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace InmobiliariaMVC.Controllers
{
    public class ContratoController : Controller
    {
        private readonly ContratoRepositorio repoContrato = new ContratoRepositorio();
        private readonly InmuebleRepositorio repoInmueble = new InmuebleRepositorio();
        private readonly InquilinoRepositorio repoInquilino = new InquilinoRepositorio();

        // GET: Contrato
        public IActionResult Index()
        {
            var lista = repoContrato.ObtenerTodos();
            return View(lista);
        }

        // GET: Contrato/Details/5
        public IActionResult Details(int id)
        {
            var contrato = repoContrato.ObtenerPorId(id);
            if (contrato == null) return NotFound();
            return View(contrato);
        }

        // GET: Contrato/Create
        public IActionResult Create()
        {
            ViewBag.Inquilinos = new SelectList(repoInquilino.ObtenerTodos(), "id_inquilino", "nombre");
            ViewBag.Inmuebles = new SelectList(repoInmueble.ObtenerTodos(), "id_inmueble", "direccion");
            return View();
        }

        // POST: Contrato/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Contrato contrato)
        {
            if (ModelState.IsValid)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
                var filas = repoContrato.Alta(contrato, userId);
                if (filas > 0) return RedirectToAction(nameof(Index));
            }

            // recargo listas si hay error
            ViewBag.Inquilinos = new SelectList(repoInquilino.ObtenerTodos(), "id_inquilino", "nombre", contrato.id_inquilino);
            ViewBag.Inmuebles = new SelectList(repoInmueble.ObtenerTodos(), "id_inmueble", "direccion", contrato.id_inmueble);
            return View(contrato);
        }

        // GET: Contrato/Edit/5
        public IActionResult Edit(int id)
        {
            var contrato = repoContrato.ObtenerPorId(id);
            if (contrato == null) return NotFound();

            ViewBag.Inquilinos = new SelectList(repoInquilino.ObtenerTodos(), "id_inquilino", "nombre", contrato.id_inquilino);
            ViewBag.Inmuebles = new SelectList(repoInmueble.ObtenerTodos(), "id_inmueble", "direccion", contrato.id_inmueble);
            return View(contrato);
        }

        // POST: Contrato/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Contrato contrato)
        {
            if (id != contrato.id_contrato) return BadRequest();

            if (ModelState.IsValid)
            {
                var filas = repoContrato.Modificacion(contrato);
                if (filas > 0) return RedirectToAction(nameof(Index));
            }

            // recargo listas si hay error
            ViewBag.Inquilinos = new SelectList(repoInquilino.ObtenerTodos(), "id_inquilino", "nombre", contrato.id_inquilino);
            ViewBag.Inmuebles = new SelectList(repoInmueble.ObtenerTodos(), "id_inmueble", "direccion", contrato.id_inmueble);
            return View(contrato);
        }

        // GET: Contrato/Delete/5
        public IActionResult Delete(int id)
        {
            var contrato = repoContrato.ObtenerPorId(id);
            if (contrato == null) return NotFound();
            return View(contrato);
        }

        // POST: Contrato/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var filas = repoContrato.Baja(id);
            if (filas > 0) return RedirectToAction(nameof(Index));
            return RedirectToAction(nameof(Delete), new { id });
        }

        // POST: Contrato/Terminar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Terminar(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            repoContrato.TerminarContrato(id, userId);
            return RedirectToAction(nameof(Index));
        }
    }
}
