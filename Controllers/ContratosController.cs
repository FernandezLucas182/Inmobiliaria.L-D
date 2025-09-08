using Microsoft.AspNetCore.Mvc;
using InmobiliariaMVC.Models;

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

    
        // GET: Contrato/Create
        public IActionResult Alta()
        {
            // Para llenar los combos
            ViewBag.Inmuebles = repoInmueble.ObtenerTodos();
            ViewBag.Inquilinos = repoInquilino.ObtenerTodos();

            return View();
        }

        // POST: Contrato/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Contrato contrato)
        {
            if (ModelState.IsValid)
            {
                var filas = repoContrato.Alta(contrato);
                if (filas > 0)
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            // Si falla, recargar combos
            ViewBag.Inmuebles = repoInmueble.ObtenerTodos();
            ViewBag.Inquilinos = repoInquilino.ObtenerTodos();

            return View(contrato);
        }

        // GET: Contrato/Edit/5
        public IActionResult Edit(int id)
        {
            var contrato = repoContrato.ObtenerPorId(id);
            if (contrato == null)
            {
                return NotFound();
            }

            ViewBag.Inmuebles = repoInmueble.ObtenerTodos();
            ViewBag.Inquilinos = repoInquilino.ObtenerTodos();

            return View(contrato);
        }

        // POST: Contrato/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Contrato contrato)
        {
            if (id != contrato.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var filas = repoContrato.Modificacion(contrato);
                if (filas > 0)
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            ViewBag.Inmuebles = repoInmueble.ObtenerTodos();
            ViewBag.Inquilinos = repoInquilino.ObtenerTodos();

            return View(contrato);
        }

        // GET: Contrato/Delete/5
        public IActionResult Delete(int id)
        {
            var contrato = repoContrato.ObtenerPorId(id);
            if (contrato == null)
            {
                return NotFound();
            }
            return View(contrato);
        }

        // POST: Contrato/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var filas = repoContrato.Baja(id);
            if (filas > 0)
            {
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Delete), new { id });
        }
    }
}
