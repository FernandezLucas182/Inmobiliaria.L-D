using InmobiliariaMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        // ====================== ADMIN ======================
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var list = _repo.GetAll();
            return View(list);
        }

        #region CREATE
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Usuario model, string password)
        {
            if (!ModelState.IsValid) return View(model);

            if (string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "La contraseña es obligatoria.");
                return View(model);
            }

            var existing = _repo.GetByEmail(model.email);
            if (existing != null)
            {
                ModelState.AddModelError("", "Ya existe un usuario con ese email.");
                return View(model);
            }

            model.password_hash = _pwdHasher.HashPassword(model, password);

            // Imagen por defecto
            if (string.IsNullOrEmpty(model.avatar_path))
                model.avatar_path = "/images/imgdef.png";

            _repo.Create(model);
            return RedirectToAction("Index");
        }
        #endregion

        #region EDIT
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            var u = _repo.GetById(id);
            if (u == null) return NotFound();
            return View(u);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Usuario model)
        {
            if (!ModelState.IsValid) return View(model);

            var u = _repo.GetById(model.id_usuario);
            if (u == null) return NotFound();

            // Solo se permite cambiar estos campos
            u.nombre = model.nombre;
            u.apellido = model.apellido;
            u.email = model.email;
            u.rol = model.rol;

            _repo.Update(u);
            return RedirectToAction("Index");
        }
        #endregion

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
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _repo.Delete(id);
            return RedirectToAction("Index");
        }

        // ====================== PERFIL USUARIO ======================
        [Authorize(Roles = "Admin,Empleado")]
        public IActionResult MiPerfil()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var usuario = _repo.GetById(userId);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        [Authorize(Roles = "Admin,Empleado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ActualizarPerfil(Usuario model, string NuevaContraseña, IFormFile Avatar, bool EliminarAvatar)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            if (model.id_usuario != userId) return Unauthorized();

            var usuario = _repo.GetById(userId);
            if (usuario == null) return NotFound();

            usuario.nombre = model.nombre;
            usuario.apellido = model.apellido;

            // Avatar
            if (EliminarAvatar)
            {
                if (!string.IsNullOrEmpty(usuario.avatar_path) && usuario.avatar_path.StartsWith("/uploads/"))
                {
                    var rutaAvatar = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", usuario.avatar_path.TrimStart('/'));
                    if (System.IO.File.Exists(rutaAvatar)) System.IO.File.Delete(rutaAvatar);
                }
                usuario.avatar_path = "/images/imgdef.png";
            }

            if (Avatar != null)
            {
                var fileName = $"{Guid.NewGuid()}_{Avatar.FileName}";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    Avatar.CopyTo(stream);
                }
                usuario.avatar_path = "/uploads/" + fileName;
            }

            // Contraseña
            if (!string.IsNullOrEmpty(NuevaContraseña))
            {
                usuario.password_hash = _pwdHasher.HashPassword(usuario, NuevaContraseña);
            }

            _repo.Update(usuario);
            return RedirectToAction("MiPerfil");
        }
    }
}
