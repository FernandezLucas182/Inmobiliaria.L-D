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
            CargarListas();
            return View();
        }

        // POST: Contrato/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Contrato contrato)
        {
            if (!ModelState.IsValid)
            {
                CargarListas();
                return View(contrato);
            }

            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
                repoContrato.Alta(contrato, userId);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Ocurrió un error al crear el contrato.");
            }

            CargarListas();
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
                try
                {
                    var filas = repoContrato.Modificacion(contrato);
                    if (filas > 0) return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Ocurrió un error al modificar el contrato.");
                }
            }

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
            return RedirectToAction(nameof(Index));
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

        // Método auxiliar para recargar dropdowns
        private void CargarListas()
        {
            ViewBag.Inquilinos = new SelectList(repoInquilino.ObtenerTodos(), "id_inquilino", "nombre");
            ViewBag.Inmuebles = new SelectList(repoInmueble.ObtenerTodos(), "id_inmueble", "direccion");
        }
    }
}
