using Newtonsoft.Json;
using Tarea3_Core.Models;

namespace Tarea3_Core.Services
{
    public class BookService
    {
        private readonly string _googleBooksApiKey = "AIzaSyBZECIblxUuBnNHqoNsmUbVzY2Ed1eKkMY"; 
        private readonly HttpClient _httpClient;

        public BookService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Método para buscar libros en Google Books API
        public async Task<List<Book>> SearchBooksAsync(string query)
        {
            var url = $"https://www.googleapis.com/books/v1/volumes?q={query}&key={_googleBooksApiKey}";
            var response = await _httpClient.GetStringAsync(url); // Realiza la llamada a la API
            var result = JsonConvert.DeserializeObject<GoogleBooksResponse>(response);  // Deserializa la respuesta

            // Devuelve la lista de libros procesada
            return result.Items.Select(item => new Book
            {
                Title = item.VolumeInfo.Title,
                Author = string.Join(", ", item.VolumeInfo.Authors),
                Description = item.VolumeInfo.Description,
                PublishedDate = item.VolumeInfo.PublishedDate,
                ImageUrl = item.VolumeInfo.ImageLinks?.Thumbnail  // Si hay una imagen
            }).ToList();
        }
    }
}
