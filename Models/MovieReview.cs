using System.ComponentModel.DataAnnotations;

namespace Fall2025_Project3._1_Loswald.Models
{
    public class MovieReview
    {
        public string ReviewText { get; set; } = string.Empty;
        public double SentimentScore { get; set; }
        public string SentimentLabel { get; set; } = string.Empty;
        public double PositiveScore { get; set; }
        public double NegativeScore { get; set; }
        public double NeutralScore { get; set; }
        public double CompoundScore { get; set; }
    }

    public class MovieReviewsResult
    {
        public List<MovieReview> Reviews { get; set; } = new List<MovieReview>();
        public double AverageSentiment { get; set; }
        public string OverallSentimentLabel { get; set; } = string.Empty;
        public string MovieTitle { get; set; } = string.Empty;
        public List<string> ActorNames { get; set; } = new List<string>();
    }
}