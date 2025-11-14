using System.ComponentModel.DataAnnotations;

namespace Fall2025_Project3._1_Loswald.Models
{
    public class MovieActor
    {
        public int MovieId { get; set; }
        public virtual Movie Movie { get; set; } = null!;
        
        public int ActorId { get; set; }
        public virtual Actor Actor { get; set; } = null!;
    }
}