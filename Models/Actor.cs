using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fall2025_Project3._1_Loswald.Models
{
    public class Actor
    {
        public int ID { get; set; }
        
        [Required]
        [Display(Name = "Name")]
        public required string Name { get; set; }
        
        [Display(Name = "IMDB Link")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        public string? IMDB_link { get; set; }
        
        [Display(Name = "Gender")]
        public string? Gender { get; set; }
        
        [Display(Name = "Age")]
        [Range(1, 120, ErrorMessage = "Please enter a valid age between 1 and 120")]
        public int Age { get; set; }
        
        [Display(Name = "Photo")]
        public byte[]? Photo { get; set; }
        
        [NotMapped]
        [Display(Name = "Photo Image")]
        public IFormFile? PhotoFile { get; set; }
        
        // Navigation property for many-to-many relationship with Movies
        public virtual ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
    }
}
