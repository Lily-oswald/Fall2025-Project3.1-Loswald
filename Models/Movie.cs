
namespace Fall2025_Project3._1_Loswald.Models
{
    public class Movie
    {
        public required string Title { get; set; }
        public string? IMDB_link { get; set; }
        public string? genre { get; set; }
        public int ReleaseYear { get; set; }
        public int ID { get; set; }

    }
}