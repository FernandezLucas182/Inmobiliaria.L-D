using InmobiliariaMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace InmobiliariaMVC.Controllers
{
    [Authorize]
    public class UsuariosController : Controller
    {
        private readonly UsuarioRepositorio _repo;
        private readonly PasswordHasher<Usuario> _pwdHasher = new PasswordHasher<Usuario>();

        public UsuariosController(UsuarioRepositorio repo)
        {
            _repo = repo;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var list = _repo.GetAll();
            return View(list);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create() => View(new Usuario());

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Create(Usuario model, string password)
        {
            if (!ModelState.IsValid) return View(model);
            if (string.IsNullOrWhiteSpace(password)) { ModelState.AddModelError("", "La contraseña es obligatoria."); return View(model); }

            var existing = _repo.GetByEmail(model.email);
            if (existing != null) { ModelState.AddModelError("", "Ya existe un usuario con ese email."); return View(model); }

            var ph = _pwdHasher.HashPassword(model, password);
            model.password_hash = ph;
            _repo.Create(model);
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            var u = _repo.GetById(id);
            if (u == null) return NotFound();
            return View(u);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Edit(Usuario model)
        {
            if (!ModelState.IsValid) return View(model);
            var u = _repo.GetById(model.id_usuario);
            if (u == null) return NotFound();
            // si el admin quiere, puede cambiar rol aquí
            u.email = model.email;
            u.nombre = model.nombre;
            u.apellido = model.apellido;
            u.rol = model.rol;
            u.avatar_path = model.avatar_path;
            _repo.Update(u);
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Details(int id)
        {
            var u = _repo.GetById(id);
            if (u == null) return NotFound();
            return View(u);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var u = _repo.GetById(id);
            if (u == null) return NotFound();
            return View(u);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _repo.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
