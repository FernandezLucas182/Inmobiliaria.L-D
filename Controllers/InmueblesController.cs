using Microsoft.AspNetCore.Mvc;
using InmobiliariaMVC.Models;

namespace InmobiliariaMVC.Controllers
{
    public class InmuebleController : Controller
    {
        private readonly InmuebleRepositorio repositorio;

        public InmuebleController()
        {
            repositorio = new InmuebleRepositorio();
        }

        // GET: Inmueble
        public IActionResult Index()
        {
            var lista = repositorio.ObtenerTodos();
            return View(lista);
        }

        // GET: Inmueble/Details/5
        public IActionResult Details(int id)
        {
            var inmueble = repositorio.ObtenerPorId(id);
            if (inmueble == null)
            {
                return NotFound();
            }
            return View(inmueble);
        }

        // GET: Inmueble/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Inmueble/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Inmueble inmueble)
        {
            if (ModelState.IsValid)
            {
                var filasAfectadas = repositorio.Crear(inmueble);
                if (filasAfectadas > 0)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(inmueble);
        }

        // GET: Inmueble/Edit/5
        public IActionResult Edit(int id)
        {
            var inmueble = repositorio.ObtenerPorId(id);
            if (inmueble == null)
            {
                return NotFound();
            }
            return View(inmueble);
        }

        // POST: Inmueble/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Inmueble inmueble)
        {
            if (id != inmueble.IdInmueble)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var filasAfectadas = repositorio.Editar(inmueble);
                if (filasAfectadas > 0)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(inmueble);
        }

        // GET: Inmueble/Delete/5
        public IActionResult Delete(int id)
        {
            var inmueble = repositorio.ObtenerPorId(id);
            if (inmueble == null)
            {
                return NotFound();
            }
            return View(inmueble);
        }

        // POST: Inmueble/Delete/5 (Baja lógica)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var filasAfectadas = repositorio.Baja(id);
            if (filasAfectadas > 0)
            {
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Delete), new { id });
        }

        // POST: Inmueble/ToggleEstado/5
        [HttpPost]
        public IActionResult ToggleEstado(int id, bool habilitar)
        {
            int filasAfectadas = 0;

            if (habilitar)
            {
                filasAfectadas = repositorio.Habilitar(id);
            }
            else
            {
                filasAfectadas = repositorio.Baja(id);
            }

            if (filasAfectadas > 0)
            {
                return RedirectToAction(nameof(Index));
            }

            return BadRequest();
        }


    }
}