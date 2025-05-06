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
            var response = await _httpClient.GetStringAsync(url);
            var result = JsonConvert.DeserializeObject<GoogleBooksResponse>(response);

            if (result.Items == null)
            {
                return new List<Book>(); // Retornar una lista vacía si no hay resultados
            }

            return result.Items.Select(item => new Book
            {
                Title = item.VolumeInfo.Title,
                Author = string.Join(", ", item.VolumeInfo.Authors ?? new List<string>()), // Proteger Authors también
                Description = item.VolumeInfo.Description,
                PublishedDate = item.VolumeInfo.PublishedDate,
                ImageUrl = item.VolumeInfo.ImageLinks?.Thumbnail,
                InfoLink = item.VolumeInfo.InfoLink
            }).ToList();
        }

    }
}
