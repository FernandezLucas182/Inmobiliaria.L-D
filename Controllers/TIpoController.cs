using Microsoft.AspNetCore.Mvc;
using InmobiliariaMVC.Models;

namespace InmobiliariaMVC.Controllers
{
    public class TipoController : Controller
    {
        private readonly TipoRepositorio repo = new TipoRepositorio();

        // GET: Tipo
        public IActionResult Index()
        {
            var tipos = repo.ObtenerTodos();
            return View(tipos);
        }

        // GET: Tipo/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tipo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Tipo tipo)
        {
            if (ModelState.IsValid)
            {
                repo.Crear(tipo);
                return RedirectToAction(nameof(Index));
            }
            return View(tipo);
        }

        // GET: Tipo/Edit/5
        public IActionResult Edit(int id)
        {
            var tipo = repo.ObtenerPorId(id);
            if (tipo == null)
                return NotFound();

            return View(tipo);
        }

        // POST: Tipo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Tipo tipo)
        {
            if (id != tipo.id_tipo)
                return BadRequest();

            if (ModelState.IsValid)
            {
                repo.Editar(tipo);
                return RedirectToAction(nameof(Index));
            }

            return View(tipo);
        }

        // GET: Tipo/Delete/5
        public IActionResult Delete(int id)
        {
            var tipo = repo.ObtenerPorId(id);
            if (tipo == null)
                return NotFound();

            return View(tipo);
        }

        // POST: Tipo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            repo.Eliminar(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
