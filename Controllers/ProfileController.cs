using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Tarea3_Core.Data;
using Tarea3_Core.Models;
using System.Linq;
using System.IO;
using System;
using Tarea3_Core.Models.ViewModels;

namespace Tarea3_Core.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProfileController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Profile()
        {
            int? userId = HttpContext.Session.GetInt32("UsuarioId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var usuario = _context.Usuarios
                .Where(u => u.Id == userId)
                .Select(u => new UsuarioViewModel
                {
                    Id = u.Id,
                    Nombre = u.Nombre,
                    Correo = u.Correo,
                    RolNombre = u.Rol != null ? u.Rol.Nombre : "Sin Rol",
                    Image = u.Image
                })
                .FirstOrDefault();

            if (usuario == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            return View(usuario);
        }


        [HttpPost]
        public IActionResult SubirFoto(IFormFile foto)
        {
            int? userId = HttpContext.Session.GetInt32("UsuarioId");
            if (userId == null || foto == null)
            {
                return RedirectToAction("Profile");
            }

            var usuario = _context.Usuarios.Find(userId);
            if (usuario == null) return NotFound();

            string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(foto.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                foto.CopyTo(fileStream);
            }

            usuario.Image = "/uploads/" + uniqueFileName;
            _context.SaveChanges();

            return RedirectToAction("Profile");
        }
    }
}
