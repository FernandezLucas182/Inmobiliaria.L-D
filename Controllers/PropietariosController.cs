using Microsoft.AspNetCore.Mvc;
using InmobiliariaMVC.Models;
using Microsoft.AspNetCore.Authorization;

namespace InmobiliariaMVC.Controllers
{
    [Authorize]
    public class PropietariosController : Controller
    {
       
        private readonly PropietarioRepositorio repo = new PropietarioRepositorio();
        [Authorize(Roles = "Admin,Empleado")]
        public IActionResult Index()
        {
            var lista = repo.ObtenerTodos();
            return View(lista);
        }

        // GET: Propietarios/Create
        // Muestra el formulario para dar de alta un propietario
        [Authorize(Roles = "Admin,Empleado")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Propietarios/Create
        // Recibe los datos del formulario y guarda un nuevo propietario
        [Authorize(Roles = "Admin,Empleado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Propietario propietario)
        {
            if (ModelState.IsValid)
            {
                repo.Alta(propietario);
                return RedirectToAction(nameof(Index));
            }
            // Si hay errores de validación, vuelve a mostrar el formulario
            return View(propietario);
        }

        // GET: Propietarios/Edit/5
        // Muestra el formulario para editar un propietario existente
        [Authorize(Roles = "Admin,Empleado")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var propietario = repo.ObtenerPorId(id);
            if (propietario == null)
            {
                return NotFound();
            }
            return View(propietario);
        }

        // POST: Propietarios/Edit/5
        // Recibe los datos modificados y actualiza el propietario en la base
        [Authorize(Roles = "Admin,Empleado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Propietario propietario)
        {
            if (ModelState.IsValid)
            {
                repo.Editar(propietario);
                return RedirectToAction(nameof(Index));
            }
            return View(propietario);
        }

        // POST: Propietarios/Delete/5
        // Borra un propietario directamente desde la lista (Index)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            repo.Borrar(id); 
            return RedirectToAction(nameof(Index)); // vuelve a mostrar la lista actualizada
        }
    }
}
