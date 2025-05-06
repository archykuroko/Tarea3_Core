namespace Tarea3_Core.Models
{
    public class GoogleBooksResponse
    {
        public List<GoogleBookItem> Items { get; set; }
    }

    public class GoogleBookItem
    {
        public GoogleBookVolumeInfo VolumeInfo { get; set; }
    }

    public class GoogleBookVolumeInfo
    {
        public string Title { get; set; }
        public List<string> Authors { get; set; }
        public string Description { get; set; }
        public string PublishedDate { get; set; }
        public GoogleBookImageLinks ImageLinks { get; set; }
        public string InfoLink { get; set; }
    }

    public class GoogleBookImageLinks
    {
        public string Thumbnail { get; set; }
    }

}
