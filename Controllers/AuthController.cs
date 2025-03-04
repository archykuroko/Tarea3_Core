using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Tarea3_Core.Data;
using Tarea3_Core.Models;

namespace Tarea3_Core.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===================== REGISTRO =====================
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Message = "❌ Datos incorrectos.";
                return View(model); // 🔹 Ahora devuelve el modelo correcto a la vista
            }

            // 🔹 Convertir RegisterViewModel a Usuarios antes de guardar en la BD
            Usuarios usuario = new Usuarios
            {
                Nombre = model.Nombre,
                Correo = model.Correo,
                Contraseña = HashPassword(model.Contraseña), // 🔹 Hashear la contraseña
                RolId = model.RolId // Asigna el RolId desde RegisterViewModel
            };

            try
            {
                _context.Usuarios.Add(usuario);
                _context.SaveChanges();
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ViewBag.Message = "❌ Error en la base de datos: " + ex.Message;
                return View(model); // 🔹 Devuelve el modelo correcto en caso de error
            }
        }


        // ===================== LOGIN =====================
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string correo, string contraseña)
        {
            if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(contraseña))
            {
                ViewBag.Error = "❌ Correo y contraseña son obligatorios";
                return View();
            }

            var usuario = _context.Usuarios.FirstOrDefault(u => u.Correo == correo);

            if (usuario != null && VerifyPassword(contraseña, usuario.Contraseña))
            {
                // ✅ Guardar sesión
                HttpContext.Session.SetInt32("UsuarioId", usuario.Id);
                HttpContext.Session.SetString("UsuarioNombre", usuario.Nombre);
                HttpContext.Session.SetString("Correo", usuario.Correo);
                HttpContext.Session.SetInt32("RolId", usuario.RolId);

                // ✅ Crear lista de claims para autenticación con cookies
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario.Nombre),
                    new Claim(ClaimTypes.Email, usuario.Correo),
                    new Claim(ClaimTypes.Role, usuario.RolId == 1 ? "Administrador" : "Usuario")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true
                };

                // ✅ Iniciar autenticación con cookies
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                // ✅ Redirigir según el rol
                if (usuario.RolId == 1)
                {
                    return RedirectToAction("Index", "Admin"); // 🔹 Administrador
                }
                else
                {
                    return RedirectToAction("Index", "Home"); // 🔹 Usuario normal
                }
            }

            ViewBag.Error = "❌ Correo o contraseña incorrectos";
            return View();
        }

        // ===================== HASH CONTRASEÑA =====================
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        private bool VerifyPassword(string inputPassword, string storedPassword)
        {
            return HashPassword(inputPassword) == storedPassword;
        }

        // ===================== LOGOUT =====================
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
