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

        [Authorize(Roles = "Admin")]

        #region CREATE
        public IActionResult Create(Usuario model, string password)
{
    if (!ModelState.IsValid) return View(model);

    // 👇 Imagen por defecto si no tiene avatar
    if (string.IsNullOrEmpty(model.avatar_path))
    {
        model.avatar_path = "/images/imgdef.png";
    }

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

    var ph = _pwdHasher.HashPassword(model, password);
    model.password_hash = ph;

    _repo.Create(model);
    return RedirectToAction("Index");
}
        #endregion
 

        [Authorize(Roles = "Admin")]
        [HttpPost]      
        public IActionResult Edit(Usuario model)
        {
            if (!ModelState.IsValid) return View(model);
            var u = _repo.GetById(model.id_usuario);
            if (u == null) return NotFound();

            // Admin puede cambiar rol
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

        // ====================== PERFIL USUARIO ======================
        public IActionResult MiPerfil()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var usuario = _repo.GetById(userId);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

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
                if (!string.IsNullOrEmpty(usuario.avatar_path))
                {
                    var rutaAvatar = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", usuario.avatar_path);
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
                usuario.avatar_path = fileName;
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
