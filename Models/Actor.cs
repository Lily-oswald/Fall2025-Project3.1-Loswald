using System.ComponentModel.DataAnnotations;

namespace Fall2025_Project3._1_Loswald.Models
{
    public class Actor
    {
        [Key] // Primary key property
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Gender { get; set; }

        public int Age { get; set; }

        [Display(Name = "IMDB Link")]
        public string? IMDB_link { get; set; }
    }
}
