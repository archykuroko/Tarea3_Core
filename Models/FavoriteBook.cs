using System.ComponentModel.DataAnnotations;

namespace Tarea3_Core.Models
{
    public class FavoriteBook
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; } = 0; // Asignamos un valor predeterminado de 0

        public string Title { get; set; } // Este puede ser NULL
        public string Author { get; set; } // Este también puede ser NULL
        public string Description { get; set; } // Este también puede ser NULL
        public string PublishedDate { get; set; } // Este también puede ser NULL
        public string ImageUrl { get; set; }
        public string InfoLink { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
