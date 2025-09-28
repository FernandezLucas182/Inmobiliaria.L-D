using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using InmobiliariaMVC.Models;
using Microsoft.AspNetCore.Authorization;

namespace InmobiliariaMVC.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UsuarioRepositorio _repo;
        private readonly PasswordHasher<Usuario> _pwdHasher = new PasswordHasher<Usuario>();
        private readonly IWebHostEnvironment _env;

        public AccountController(UsuarioRepositorio repo, IWebHostEnvironment env)
        {
            _repo = repo;
            _env = env;
        }

        // ================= LOGIN =================
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError("", "Email y contraseña son obligatorios.");
                return View(model);
            }

            var user = _repo.GetByEmail(model.Email!);
            if (user == null)
            {
                ModelState.AddModelError("", "Email o contraseña inválidos.");
                return View(model);
            }

            var verify = _pwdHasher.VerifyHashedPassword(user, user.password_hash, model.Password!);
            if (verify == PasswordVerificationResult.Success)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.id_usuario.ToString()),
                    new Claim(ClaimTypes.Name, $"{user.nombre} {user.apellido}"),
                    new Claim(ClaimTypes.Email, user.email),
                    new Claim(ClaimTypes.Role, user.rol)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Email o contraseña inválidos.");
            return View(model);
        }

        // ================= LOGOUT =================
        [Authorize(Roles = "Admin,Empleado")]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        // ================= PERFIL =================
        [Authorize(Roles = "Admin,Empleado")]
        [HttpGet]
        public IActionResult Profile()
        {
            var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var user = _repo.GetById(id);
            if (user == null) return NotFound();
            return View(user);
        }

        [Authorize(Roles = "Admin,Empleado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Profile(
            Usuario model,
            IFormFile? avatarFile,
            bool EliminarAvatar = false,
            string? currentPassword = null,
            string? newPassword = null,
            string? newPasswordConfirm = null)
        {
            var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var user = _repo.GetById(id);
            if (user == null) return NotFound();

            // Actualizar datos básicos
            user.nombre = model.nombre;
            user.apellido = model.apellido;
            user.email = model.email;

            // Eliminar avatar si se marcó
            if (EliminarAvatar && !string.IsNullOrEmpty(user.avatar_path))
            {
                var physical = Path.Combine(_env.WebRootPath, user.avatar_path.TrimStart('/', '\\'));
                if (System.IO.File.Exists(physical)) System.IO.File.Delete(physical);
                user.avatar_path = null;
            }

            // Subir nuevo avatar
            if (avatarFile != null && avatarFile.Length > 0)
            {
                var ext = Path.GetExtension(avatarFile.FileName).ToLowerInvariant();
                var allowedExt = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                const long maxSize = 2 * 1024 * 1024; // 2 MB

                if (!allowedExt.Contains(ext))
                {
                    ModelState.AddModelError("", "Solo se permiten imágenes (.jpg, .jpeg, .png, .gif).");
                    return View(user);
                }

                if (avatarFile.Length > maxSize)
                {
                    ModelState.AddModelError("", "El tamaño máximo del avatar es 2 MB.");
                    return View(user);
                }

                var uploads = Path.Combine(_env.WebRootPath, "uploads", "avatars");
                if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploads, fileName);

                using var fs = new FileStream(filePath, FileMode.Create);
                avatarFile.CopyTo(fs);

                // Eliminar avatar anterior
                if (!string.IsNullOrEmpty(user.avatar_path))
                {
                    var old = Path.Combine(_env.WebRootPath, user.avatar_path.TrimStart('/', '\\'));
                    if (System.IO.File.Exists(old)) System.IO.File.Delete(old);
                }

                user.avatar_path = $"/uploads/avatars/{fileName}";
            }

            // Cambiar contraseña si se ingresó
            if (!string.IsNullOrEmpty(currentPassword) && !string.IsNullOrEmpty(newPassword) && !string.IsNullOrEmpty(newPasswordConfirm))
            {
                var verify = _pwdHasher.VerifyHashedPassword(user, user.password_hash, currentPassword);
                if (verify != PasswordVerificationResult.Success)
                {
                    ModelState.AddModelError("", "Contraseña actual incorrecta.");
                    return View(user);
                }
                if (newPassword != newPasswordConfirm)
                {
                    ModelState.AddModelError("", "La nueva contraseña y su confirmación no coinciden.");
                    return View(user);
                }

                var newHash = _pwdHasher.HashPassword(user, newPassword);
                _repo.UpdatePassword(user.id_usuario, newHash);
            }

            _repo.Update(user);

            ViewBag.ProfileMessage = "Perfil actualizado correctamente.";
            if (!string.IsNullOrEmpty(newPassword)) ViewBag.PasswordMessage = "Contraseña cambiada correctamente.";

            return View(user);
        }

        // ================= ACCESO DENEGADO =================
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
