using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using InmobiliariaMVC.Models;
using Microsoft.AspNetCore.Authorization;

namespace InmobiliariaMVC.Controllers
{
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

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string email, string password, string? returnUrl = null)
        {
            var user = _repo.GetByEmail(email);
            if (user == null)
            {
                ModelState.AddModelError("", "Email o contraseña inválidos.");
                return View();
            }

            var verify = _pwdHasher.VerifyHashedPassword(user, user.password_hash, password);
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
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        [Authorize]
        [HttpGet]
        public IActionResult Profile()
        {
            var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var user = _repo.GetById(id);
            return View(user);
        }

        [Authorize]
        [HttpPost]
        [Authorize]
        [HttpPost]
        public IActionResult Profile(
    Usuario model,
    IFormFile avatarFile,
    bool EliminarAvatar = false,
    string? currentPassword = null,
    string? newPassword = null,
    string? newPasswordConfirm = null)
        {
            var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var user = _repo.GetById(id);
            if (user == null) return NotFound();

            // actualizar datos básicos
            user.nombre = model.nombre;
            user.apellido = model.apellido;
            user.email = model.email;

            // eliminar avatar si se marcó
            if (EliminarAvatar && !string.IsNullOrEmpty(user.avatar_path))
            {
                var physical = Path.Combine(_env.WebRootPath, user.avatar_path.TrimStart('/', '\\'));
                if (System.IO.File.Exists(physical)) System.IO.File.Delete(physical);
                user.avatar_path = null;
            }

            // subir nuevo avatar con validaciones
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

                // eliminar avatar anterior si existe
                if (!string.IsNullOrEmpty(user.avatar_path))
                {
                    var old = Path.Combine(_env.WebRootPath, user.avatar_path.TrimStart('/', '\\'));
                    if (System.IO.File.Exists(old)) System.IO.File.Delete(old);
                }

                user.avatar_path = $"/uploads/avatars/{fileName}";
            }

            // cambiar contraseña si se ingresó
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

                user.password_hash = _pwdHasher.HashPassword(user, newPassword);
            }

            _repo.Update(user);
            return RedirectToAction("Profile");
        }


        [Authorize]
        [HttpPost]
        public IActionResult DeleteAvatar()
        {
            var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var user = _repo.GetById(id);
            if (user == null) return NotFound();

            if (!string.IsNullOrEmpty(user.avatar_path))
            {
                var physical = Path.Combine(_env.WebRootPath, user.avatar_path.TrimStart('/', '\\'));
                if (System.IO.File.Exists(physical)) System.IO.File.Delete(physical);
            }

            user.avatar_path = null;
            _repo.Update(user);
            return RedirectToAction("Profile");
        }

        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword() => View();

        [Authorize]
        [HttpPost]
        public IActionResult ChangePassword(string currentPassword, string newPassword, string newPasswordConfirm)
        {
            var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var user = _repo.GetById(id);
            if (user == null) return NotFound();

            var verify = _pwdHasher.VerifyHashedPassword(user, user.password_hash, currentPassword);
            if (verify != PasswordVerificationResult.Success)
            {
                ModelState.AddModelError("", "Contraseña actual incorrecta.");
                return View();
            }
            if (newPassword != newPasswordConfirm)
            {
                ModelState.AddModelError("", "La nueva contraseña y su confirmación no coinciden.");
                return View();
            }

            var newHash = _pwdHasher.HashPassword(user, newPassword);
            _repo.UpdatePassword(user.id_usuario, newHash);
            return RedirectToAction("Profile");
        }

        [AllowAnonymous]
        public IActionResult AccessDenied() => View();


        /*   public IActionResult CrearAdmin()
           {
               var user = new Usuario
               {
                   nombre = "Admin",
                   apellido = "Principal",
                   email = "admin@inmobiliaria.com",
                   rol = "Admin"
               };

               var hasher = new PasswordHasher<Usuario>();
               user.password_hash = hasher.HashPassword(user, "Admin123!"); // contraseña inicial

               _repo.Create(user); // o el método que uses para guardar
               return Content("Usuario administrador creado correctamente");
           }
       */
    }
}


