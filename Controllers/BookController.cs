using Microsoft.AspNetCore.Mvc;
using Tarea3_Core.Services; 
using Tarea3_Core.Models;  

namespace Tarea3_Core.Controllers
{
    public class BookController : Controller
    {
        private readonly BookService _bookService;

        // Constructor para inyectar BookService
        public BookController(BookService bookService)
        {
            _bookService = bookService;
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
    }
}
