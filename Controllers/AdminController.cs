using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using Tarea3_Core.Data;
using Tarea3_Core.Models;
using Tarea3_Core.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Tarea3_Core.Controllers
{
    [Authorize(Roles = "Administrador")] // 🔒 Solo los administradores pueden acceder
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 📌 1. LISTAR USUARIOS
        public IActionResult Index()
        {
            var usuarios = _context.Usuarios
                .Include(u => u.Rol) // 🔹 Carga los roles
                .Select(u => new UsuarioViewModel
                {
                    Id = u.Id,
                    Nombre = u.Nombre,
                    Correo = u.Correo,
                    RolNombre = u.Rol != null ? u.Rol.Nombre : "Sin Rol"
                })
                .ToList();

            return View(usuarios);
        }

        // 📌 2. MOSTRAR FORMULARIO DE EDICIÓN
        public IActionResult Edit(int id)
        {
            var usuario = _context.Usuarios.Find(id);
            if (usuario == null) return NotFound();

            ViewBag.Roles = _context.Roles
                .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Nombre })
                .ToList();

            return View(usuario);
        }

        // 📌 3. PROCESAR EDICIÓN
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Usuarios usuario)
        {
            var usuarioExistente = _context.Usuarios.Find(usuario.Id);
            if (usuarioExistente == null) return NotFound();

            usuarioExistente.Nombre = usuario.Nombre;
            usuarioExistente.Correo = usuario.Correo;
            usuarioExistente.RolId = usuario.RolId;

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // 📌 4. MOSTRAR FORMULARIO DE ELIMINACIÓN
        public IActionResult Delete(int id)
        {
            var usuario = _context.Usuarios.Find(id);
            if (usuario == null) return NotFound();

            return View(usuario);
        }

        // 📌 5. PROCESAR ELIMINACIÓN
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var usuario = _context.Usuarios.Find(id);
            if (usuario == null) return NotFound();

            _context.Usuarios.Remove(usuario);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // 📌 6. MOSTRAR FORMULARIO DE CREACIÓN
        public IActionResult Create()
        {
            ViewBag.Roles = _context.Roles
                .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Nombre })
                .ToList();

            return View();
        }

        // 📌 7. PROCESAR CREACIÓN
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Usuarios usuario)
        {
            if (_context.Usuarios.Any(u => u.Correo == usuario.Correo))
            {
                ViewBag.Error = "❌ Este correo ya está registrado.";
                ViewBag.Roles = _context.Roles
                    .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Nombre })
                    .ToList();
                return View(usuario);
            }

            usuario.Contraseña = HashPassword(usuario.Contraseña);
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // ✅ Método para encriptar contraseñas
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
