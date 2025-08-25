using Microsoft.AspNetCore.Mvc;
using InmobiliariaMVC.Models;

namespace InmobiliariaMVC.Controllers
{
    public class PropietariosController : Controller
    {
        private readonly PropietarioRepositorio repo = new PropietarioRepositorio();

        public IActionResult Index()
        {
            var lista = repo.ObtenerTodos();
            return View(lista);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Propietario propietario)
        {
            if (ModelState.IsValid)
            {
                repo.Alta(propietario);
                return RedirectToAction(nameof(Index));
            }
            return View(propietario);
        }
    }
}
