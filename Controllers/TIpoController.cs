using Microsoft.AspNetCore.Mvc;
using InmobiliariaMVC.Models;
using Microsoft.AspNetCore.Authorization;

namespace InmobiliariaMVC.Controllers
{
    [Authorize]
    public class TipoController : Controller

    {
        private readonly TipoRepositorio repo = new TipoRepositorio();

        // GET: Tipo
        [Authorize(Roles = "Admin,Empleado")]
        public IActionResult Index()
        {
            var tipos = repo.ObtenerTodos();
            return View(tipos);
        }

        // GET: Tipo/Create
        [Authorize(Roles = "Admin,Empleado")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tipo/Create
        [Authorize(Roles = "Admin,Empleado")]
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
        [Authorize(Roles = "Admin,Empleado")]
        public IActionResult Edit(int id)
        {
            var tipo = repo.ObtenerPorId(id);
            if (tipo == null)
                return NotFound();

            return View(tipo);
        }

        // POST: Tipo/Edit/5
        [Authorize(Roles = "Admin,Empleado")]
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



        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            repo.Eliminar(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
