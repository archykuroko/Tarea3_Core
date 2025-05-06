using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
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
                return View(model);
            }

            // 🔹 Verificar si el correo ya está registrado
            if (_context.Usuarios.Any(u => u.Correo == model.Correo))
            {
                ViewBag.Message = "❌ Este correo ya está registrado.";
                return View(model);
            }

            // 🔹 Crear el usuario con los datos proporcionados
            var usuario = new Usuarios
            {
                Nombre = model.Nombre,
                Correo = model.Correo,
                Contraseña = HashPassword(model.Contraseña),
                RolId = model.RolId,
                Image = "/uploads/default.png" // Imagen por defecto
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
                return View(model);
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
            // ✅ Verificar si el correo o la contraseña están vacíos
            if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(contraseña))
            {
                ViewBag.Error = "❌ Correo y contraseña son obligatorios.";
                return View();
            }

            try
            {
                // ✅ Depuración: Verificar si el contexto está inicializado
                if (_context.Usuarios == null)
                {
                    ViewBag.Error = "❌ Error interno: La base de datos no está conectada.";
                    return View();
                }

                // ✅ Buscar usuario en la base de datos
                var usuario = _context.Usuarios.FirstOrDefault(u => u.Correo == correo);

                // ✅ Depuración: Verificar si el usuario existe
                if (usuario == null)
                {
                    ViewBag.Error = "❌ Usuario no encontrado.";
                    return View();
                }

                // ✅ Manejar el caso de `NULL` en `Image`
                usuario.Image = string.IsNullOrEmpty(usuario.Image) ? "/uploads/default.png" : usuario.Image;

                // ✅ Verificar la contraseña
                if (!VerifyPassword(contraseña, usuario.Contraseña))
                {
                    ViewBag.Error = "❌ Correo o contraseña incorrectos.";
                    return View();
                }

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
                var authProperties = new AuthenticationProperties { IsPersistent = true };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                return usuario.RolId == 1 ? RedirectToAction("Index", "Home") : RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "❌ Error inesperado: " + ex.Message;
                return View();
            }
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
