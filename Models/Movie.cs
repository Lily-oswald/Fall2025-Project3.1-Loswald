
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fall2025_Project3._1_Loswald.Models
{
    public class Movie
    {
        public int ID { get; set; }
        
        [Required]
        [Display(Name = "Title")]
        public required string Title { get; set; }
        
        [Display(Name = "IMDB Link")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        public string? IMDB_link { get; set; }
        
        [Display(Name = "Genre")]
        public string? genre { get; set; }
        
        [Display(Name = "Release Year")]
        [Range(1888, 2030, ErrorMessage = "Please enter a valid year between 1888 and 2030")]
        public int ReleaseYear { get; set; }
        
        [Display(Name = "Poster")]
        public byte[]? Poster { get; set; }
        
        [NotMapped]
        [Display(Name = "Poster Image")]
        public IFormFile? PosterFile { get; set; }
        
        // Navigation property for many-to-many relationship with Actors
        public virtual ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
    }
}