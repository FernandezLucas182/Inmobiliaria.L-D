using Microsoft.AspNetCore.Mvc;
using InmobiliariaMVC.Models;
using Microsoft.AspNetCore.Authorization;

namespace InmobiliariaMVC.Controllers
{   
    [Authorize]
    public class InquilinosController : Controller
    {
        // Repositorio para acceder a los datos de Inquilinos
        private readonly InquilinoRepositorio repo = new InquilinoRepositorio();

        // GET: Inquilinos
        // Muestra la lista de todos los inquilinos
        [Authorize(Roles = "Admin,Empleado")]
        public IActionResult Index()
        {
            var lista = repo.ObtenerTodos();
            return View(lista);
        }

        // GET: Inquilinos/Create
        // Muestra el formulario para dar de alta un inquilino
        [Authorize(Roles = "Admin,Empleado")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Inquilinos/Create
        [Authorize(Roles = "Admin,Empleado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Inquilino inquilino)
        {
            if (ModelState.IsValid)
            {
                repo.Alta(inquilino);
                return RedirectToAction(nameof(Index));
            }
            return View(inquilino);
        }

        // GET: Inquilinos/Edit/5
        [Authorize(Roles = "Admin,Empleado")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var inquilino = repo.ObtenerPorId(id);
            if (inquilino == null)
            {
                return NotFound();
            }
            return View(inquilino);
        }

        // POST: Inquilinos/Edit/5
        [Authorize(Roles = "Admin,Empleado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Inquilino inquilino)
        {
            if (ModelState.IsValid)
            {
                repo.Modificar(inquilino);
                return RedirectToAction(nameof(Index));
            }
            return View(inquilino);
        }

        [Authorize(Roles = "Admin,Empleado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            repo.Baja(id); 
            return RedirectToAction(nameof(Index)); 
        }
    }
}
