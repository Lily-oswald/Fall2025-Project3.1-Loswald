namespace Fall2025_Project3._1_Loswald.Models
{
    public class ActorTweet
    {
        public string TweetText { get; set; } = string.Empty;
        public double SentimentScore { get; set; }
        public string SentimentLabel { get; set; } = string.Empty;
        public double PositiveScore { get; set; }
        public double NegativeScore { get; set; }
        public double NeutralScore { get; set; }
        public double CompoundScore { get; set; }
    }

    public class ActorTweetsResult
    {
        public List<ActorTweet> Tweets { get; set; } = new List<ActorTweet>();
        public double AverageSentiment { get; set; }
        public string OverallSentimentLabel { get; set; } = string.Empty;
        public string ActorName { get; set; } = string.Empty;
        public List<string> MovieTitles { get; set; } = new List<string>();
    }
}