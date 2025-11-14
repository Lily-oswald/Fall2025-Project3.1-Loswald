using Fall2025_Project3._1_Loswald.Models;
using VaderSharp2;
using System.Text.Json;

namespace Fall2025_Project3._1_Loswald.Services
{
    public interface IMovieReviewService
    {
        Task<MovieReviewsResult> GenerateMovieReviewsAsync(Movie movie, List<Actor> actors);
    }

    public class MovieReviewService : IMovieReviewService
    {
        private readonly SentimentIntensityAnalyzer _sentimentAnalyzer;
        private readonly IConfiguration _configuration;

        public MovieReviewService(IConfiguration configuration)
        {
            _configuration = configuration;
            _sentimentAnalyzer = new SentimentIntensityAnalyzer();
        }

        public Task<MovieReviewsResult> GenerateMovieReviewsAsync(Movie movie, List<Actor> actors)
        {
            try
            {
                var actorNames = actors.Select(a => a.Name).ToList();

                // For now, we'll generate sample reviews without OpenAI
                // This allows the feature to work while we can later integrate the AI API
                var generatedReviews = GenerateSampleReviews(movie, actorNames);

                // Analyze sentiment for each review
                var reviews = new List<MovieReview>();
                double totalSentiment = 0;

                foreach (var reviewText in generatedReviews)
                {
                    var sentimentResult = _sentimentAnalyzer.PolarityScores(reviewText);
                    var review = new MovieReview
                    {
                        ReviewText = reviewText,
                        SentimentScore = sentimentResult.Compound,
                        SentimentLabel = GetSentimentLabel(sentimentResult.Compound),
                        PositiveScore = sentimentResult.Positive,
                        NegativeScore = sentimentResult.Negative,
                        NeutralScore = sentimentResult.Neutral,
                        CompoundScore = sentimentResult.Compound
                    };
                    reviews.Add(review);
                    totalSentiment += sentimentResult.Compound;
                }

                var averageSentiment = totalSentiment / reviews.Count;

                var result = new MovieReviewsResult
                {
                    Reviews = reviews,
                    AverageSentiment = averageSentiment,
                    OverallSentimentLabel = GetSentimentLabel(averageSentiment),
                    MovieTitle = movie.Title,
                    ActorNames = actorNames
                };

                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                // Return a fallback result
                return Task.FromResult(CreateFallbackResult(movie, actors, ex.Message));
            }
        }

        private List<string> GenerateSampleReviews(Movie movie, List<string> actorNames)
        {
            var actorsText = actorNames.Any() ? string.Join(", ", actorNames) : "talented actors";
            var reviews = new List<string>
            {
                $"Outstanding performance by {actorsText} in {movie.Title}. This {movie.ReleaseYear} film delivers exceptional storytelling that captivates from start to finish. The {movie.genre ?? "genre"} elements are masterfully woven throughout.",
                
                $"Disappointing execution in {movie.Title}. Despite the promising cast including {actorsText}, the film fails to live up to expectations. The plot feels disjointed and the pacing drags significantly.",
                
                $"A solid entry in the {movie.genre ?? "drama"} category. {movie.Title} showcases strong performances, particularly from {actorsText}. While not groundbreaking, it's an enjoyable watch that delivers on its promises.",
                
                $"Mediocre at best. {movie.Title} ({movie.ReleaseYear}) lacks the depth needed for compelling cinema. The performances from {actorsText} feel phoned in and uninspired.",
                
                $"Brilliant filmmaking! {movie.Title} is a tour de force that showcases the incredible range of {actorsText}. This {movie.ReleaseYear} release will be remembered as a classic of the {movie.genre ?? "genre"}.",
                
                $"Average film that doesn't particularly stand out. {movie.Title} has its moments, but overall feels forgettable. The cast, including {actorsText}, does their best with limited material.",
                
                $"Absolutely fantastic! {movie.Title} exceeded all expectations with phenomenal performances from {actorsText}. The direction is crisp, the story engaging, and the production quality top-notch.",
                
                $"Poorly executed concept. {movie.Title} suffers from weak writing and direction that wastes the talent of {actorsText}. The {movie.ReleaseYear} release feels dated and uninspired.",
                
                $"Delightful surprise! {movie.Title} brings fresh perspective to familiar themes. The chemistry between {actorsText} elevates what could have been a standard {movie.genre ?? "genre"} film.",
                
                $"Boring and predictable. {movie.Title} follows every cliché in the book without adding anything new. Even the talented cast of {actorsText} can't save this uninspired {movie.ReleaseYear} release."
            };

            return reviews;
        }

        private string GetSentimentLabel(double compoundScore)
        {
            if (compoundScore >= 0.05)
                return "Positive";
            else if (compoundScore <= -0.05)
                return "Negative";
            else
                return "Neutral";
        }

        private MovieReviewsResult CreateFallbackResult(Movie movie, List<Actor> actors, string errorMessage)
        {
            var actorNames = actors.Select(a => a.Name).ToList();
            var fallbackReviews = GenerateSampleReviews(movie, actorNames);

            var reviews = fallbackReviews.Select(text =>
            {
                var sentimentResult = _sentimentAnalyzer.PolarityScores(text);
                return new MovieReview
                {
                    ReviewText = text,
                    SentimentScore = sentimentResult.Compound,
                    SentimentLabel = GetSentimentLabel(sentimentResult.Compound),
                    PositiveScore = sentimentResult.Positive,
                    NegativeScore = sentimentResult.Negative,
                    NeutralScore = sentimentResult.Neutral,
                    CompoundScore = sentimentResult.Compound
                };
            }).ToList();

            var averageSentiment = reviews.Average(r => r.SentimentScore);

            return new MovieReviewsResult
            {
                Reviews = reviews,
                AverageSentiment = averageSentiment,
                OverallSentimentLabel = GetSentimentLabel(averageSentiment),
                MovieTitle = movie.Title,
                ActorNames = actorNames
            };
        }
    }
}