using Fall2025_Project3._1_Loswald.Models;
using VaderSharp2;

namespace Fall2025_Project3._1_Loswald.Services
{
    public interface IActorTweetService
    {
        Task<ActorTweetsResult> GenerateActorTweetsAsync(Actor actor, List<string> movieTitles);
    }

    public class ActorTweetService : IActorTweetService
    {
        private readonly SentimentIntensityAnalyzer _sentimentAnalyzer;
        private readonly IConfiguration _configuration;

        public ActorTweetService(IConfiguration configuration)
        {
            _configuration = configuration;
            _sentimentAnalyzer = new SentimentIntensityAnalyzer();
        }

        public Task<ActorTweetsResult> GenerateActorTweetsAsync(Actor actor, List<string> movieTitles)
        {
            try
            {
                // Generate 20 realistic Twitter-style tweets about the actor
                var generatedTweets = GenerateSampleTweets(actor, movieTitles);

                // Analyze sentiment for each tweet
                var tweets = new List<ActorTweet>();
                double totalSentiment = 0;

                foreach (var tweetText in generatedTweets)
                {
                    var sentimentResult = _sentimentAnalyzer.PolarityScores(tweetText);
                    var tweet = new ActorTweet
                    {
                        TweetText = tweetText,
                        SentimentScore = sentimentResult.Compound,
                        SentimentLabel = GetSentimentLabel(sentimentResult.Compound),
                        PositiveScore = sentimentResult.Positive,
                        NegativeScore = sentimentResult.Negative,
                        NeutralScore = sentimentResult.Neutral,
                        CompoundScore = sentimentResult.Compound
                    };
                    tweets.Add(tweet);
                    totalSentiment += sentimentResult.Compound;
                }

                var averageSentiment = totalSentiment / tweets.Count;

                var result = new ActorTweetsResult
                {
                    Tweets = tweets,
                    AverageSentiment = averageSentiment,
                    OverallSentimentLabel = GetSentimentLabel(averageSentiment),
                    ActorName = actor.Name,
                    MovieTitles = movieTitles
                };

                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                // Return a fallback result
                return Task.FromResult(CreateFallbackResult(actor, movieTitles, ex.Message));
            }
        }

        private List<string> GenerateSampleTweets(Actor actor, List<string> movieTitles)
        {
            var moviesText = movieTitles.Any() ? string.Join(", ", movieTitles.Take(3)) : "various films";
            var tweets = new List<string>
            {
                $"Just watched {actor.Name} in {moviesText.Split(',').FirstOrDefault() ?? "their latest film"} - absolutely phenomenal performance! 🎬✨ #Acting #Movies",
                
                $"{actor.Name} is seriously underrated as an actor. Their range in {moviesText} is incredible! 👏 #Talent #Cinema",
                
                $"Unpopular opinion: {actor.Name} was miscast in {moviesText.Split(',').FirstOrDefault() ?? "that movie"}. Just didn't feel right 🤷‍♀️ #MovieTalk",
                
                $"Can we talk about {actor.Name}'s chemistry with the cast? Pure magic on screen! ✨ #MovieMagic",
                
                $"{actor.Name} carried that entire film on their shoulders. What a powerhouse performance! 💪🎭 #ActingSkills",
                
                $"Meh. {actor.Name} was okay but nothing special. Expected more tbh 😐 #MovieReview #Disappointing",
                
                $"OBSESSED with {actor.Name}'s latest work! Every scene was captivating 😍🎬 #NewFavorite #MustWatch",
                
                $"Why is everyone hyping {actor.Name}? Seriously don't get the appeal 🙄 #Overrated",
                
                $"{actor.Name} brought so much depth to their character. Emotional rollercoaster! 😭💕 #DeepActing #Feels",
                
                $"That awkward moment when {actor.Name} tries to be funny but... yikes 😬 #ActingFail #Cringe",
                
                $"Give {actor.Name} ALL the awards! That performance was simply outstanding 🏆🌟 #AwardWorthy #Excellence",
                
                $"{actor.Name} looked great but the performance felt forced. Style over substance? 💅 #Beauty #Acting",
                
                $"Can't stop thinking about {actor.Name}'s monologue. Gave me chills! 🥶✨ #PowerfulActing #Memorable",
                
                $"Hollywood needs more actors like {actor.Name}! Fresh, authentic, and talented 🙌 #FutureOfCinema #Rising",
                
                $"Maybe it's just me but {actor.Name} seems to play the same character in every movie 🤔 #OneNote #TypeCast",
                
                $"{actor.Name} + amazing script = pure cinematic gold! What a combination 🎯🎬 #PerfectMatch #Quality",
                
                $"That scene with {actor.Name} had me ugly crying in the theater 😭🎭 #EmotionalDamage #Incredible",
                
                $"Is it just me or is {actor.Name} getting better with every role? Growth! 📈✨ #Evolution #Talent",
                
                $"Not gonna lie, {actor.Name} was the only good thing about that trainwreck of a movie 🚂💥 #SilverLining",
                
                $"BREAKING: {actor.Name} spotted at coffee shop being absolutely normal. Still gorgeous though ☕😍 #CelebSighting #CasualBeauty"
            };

            return tweets;
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

        private ActorTweetsResult CreateFallbackResult(Actor actor, List<string> movieTitles, string errorMessage)
        {
            var fallbackTweets = GenerateSampleTweets(actor, movieTitles);

            var tweets = fallbackTweets.Select(text =>
            {
                var sentimentResult = _sentimentAnalyzer.PolarityScores(text);
                return new ActorTweet
                {
                    TweetText = text,
                    SentimentScore = sentimentResult.Compound,
                    SentimentLabel = GetSentimentLabel(sentimentResult.Compound),
                    PositiveScore = sentimentResult.Positive,
                    NegativeScore = sentimentResult.Negative,
                    NeutralScore = sentimentResult.Neutral,
                    CompoundScore = sentimentResult.Compound
                };
            }).ToList();

            var averageSentiment = tweets.Average(t => t.SentimentScore);

            return new ActorTweetsResult
            {
                Tweets = tweets,
                AverageSentiment = averageSentiment,
                OverallSentimentLabel = GetSentimentLabel(averageSentiment),
                ActorName = actor.Name,
                MovieTitles = movieTitles
            };
        }
    }
}