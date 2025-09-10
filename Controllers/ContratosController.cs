using Microsoft.AspNetCore.Mvc;
using InmobiliariaMVC.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace InmobiliariaMVC.Controllers
{
    public class ContratoController : Controller
    {
        private readonly ContratoRepositorio repoContrato = new ContratoRepositorio();


        // GET: Contrato
        public IActionResult Index()
        {
            var lista = repoContrato.ObtenerTodos();
            return View(lista);
        }


        // GET: Contrato/Create
        public IActionResult Create()
        {
            var repoInmueble = new InmuebleRepositorio();
            var repoInquilino = new InquilinoRepositorio();

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
                var filas = repoContrato.Alta(contrato);
                if (filas > 0)
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            // recargo listas si vuelve por error
            var repoInmueble = new InmuebleRepositorio();
            var repoInquilino = new InquilinoRepositorio();

            ViewBag.Inquilinos = new SelectList(repoInquilino.ObtenerTodos(), "id_inquilino", "nombre");
            ViewBag.Inmuebles = new SelectList(repoInmueble.ObtenerTodos(), "id_inmueble", "direccion");

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

            var repoInmueble = new InmuebleRepositorio();
            var repoInquilino = new InquilinoRepositorio();

            ViewBag.Inmuebles = new SelectList(repoInmueble.ObtenerTodos(), "id_inmueble", "direccion", contrato.InmuebleId);
            ViewBag.Inquilinos = new SelectList(repoInquilino.ObtenerTodos(), "id_inquilino", "nombre", contrato.InquilinoId);


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

            // recargo listas si hay error
            var repoInmueble = new InmuebleRepositorio();
            var repoInquilino = new InquilinoRepositorio();

            ViewBag.Inmuebles = new SelectList(repoInmueble.ObtenerTodos(), "id_inmueble", "direccion", contrato.InmuebleId);
            ViewBag.Inquilinos = new SelectList(repoInquilino.ObtenerTodos(), "id_inquilino", "nombre", contrato.InquilinoId);

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

        public IActionResult Details(int id)
        {
            var contrato = repoContrato.ObtenerPorId(id);
            if (contrato == null)
            {
                return NotFound();
            }
            return View(contrato);
        }
    }
}
