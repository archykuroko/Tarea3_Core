using Microsoft.AspNetCore.Mvc;
using Tarea3_Core.Services;
using Tarea3_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Tarea3_Core.Data;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Tarea3_Core.Controllers
{
    public class BookController : Controller
    {
        private readonly BookService _bookService;
        private readonly ApplicationDbContext _context;

        // Constructor para inyectar BookService
        public BookController(BookService bookService, ApplicationDbContext context)
        {
            _bookService = bookService;
            _context = context;
        }

        // Acción para mostrar la vista de búsqueda
        public IActionResult Index()
        {
            return View();
        }

        // Acción para buscar libros
        [HttpGet]
        public async Task<IActionResult> SearchBooks(string query)
        {
            // Si no hay consulta, simplemente muestra la vista vacía
            if (string.IsNullOrEmpty(query))
            {
                return View(new List<Book>());
            }

            // Realiza la búsqueda con el servicio de Google Books
            var books = await _bookService.SearchBooksAsync(query);
            return View(books); // Pasa los resultados a la vista
        }

        // Acción para agregar un libro a favoritos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFavorite(string title, string author, string description, string publishedDate, string imageUrl, string infoLink)
        {
            // Si no hay autenticación, asigna el valor 0 a UserId (para usuarios no autenticados)
            var userId = 0; // Usamos 0 para usuarios no autenticados

            // Asegurarse de que los campos nulos o vacíos tengan valores predeterminados
            title = string.IsNullOrEmpty(title) ? "Título no disponible" : title;
            author = string.IsNullOrEmpty(author) ? "Autor no disponible" : author;
            description = string.IsNullOrEmpty(description) ? "Descripción no disponible" : description;
            publishedDate = string.IsNullOrEmpty(publishedDate) ? "Fecha no disponible" : publishedDate;
            imageUrl = string.IsNullOrEmpty(imageUrl) ? "/path/to/default-image.jpg" : imageUrl;
            infoLink = string.IsNullOrEmpty(infoLink) ? "#" : infoLink;

            // Crear el libro favorito con los valores establecidos
            var favoriteBook = new FavoriteBook
            {
                UserId = userId, // Si el usuario no está autenticado, se mantiene el valor 0
                Title = title,
                Author = author,
                Description = description,
                PublishedDate = publishedDate,
                ImageUrl = imageUrl,
                InfoLink = infoLink,
                DateAdded = DateTime.UtcNow // Establecemos la fecha actual
            };

            // Guardar el libro favorito en la base de datos
            _context.FavoriteBooks.Add(favoriteBook);
            await _context.SaveChangesAsync();

            // Redirigir al listado de favoritos
            return RedirectToAction("Favorite");
        }



        // Acción para mostrar los libros favoritos
        public IActionResult Favorite()
        {
            // Recuperar todos los libros favoritos
            var favoriteBooks = _context.FavoriteBooks.ToList();

            return View(favoriteBooks);  // Ya no necesitamos aplicar valores predeterminados, ya se guardan en la base de datos tal como están.
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFavorite(int bookId)
        {
            // Buscar el libro en la base de datos
            var bookToDelete = await _context.FavoriteBooks.FindAsync(bookId);

            if (bookToDelete == null)
            {
                // Si el libro no existe, redirigimos con un mensaje de error
                return NotFound();
            }

            // Eliminar el libro
            _context.FavoriteBooks.Remove(bookToDelete);
            await _context.SaveChangesAsync();

            // Redirigir de nuevo a la página de favoritos
            return RedirectToAction("Favorite");
        }
    }
}
