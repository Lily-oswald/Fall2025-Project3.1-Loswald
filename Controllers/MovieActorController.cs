using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fall2025_Project3._1_Loswald.Data;
using Fall2025_Project3._1_Loswald.Models;

namespace Fall2025_Project3._1_Loswald.Controllers
{
    public class MovieActorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MovieActorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MovieActor Management Page
        public async Task<IActionResult> Index()
        {
            var movieActors = await _context.MovieActor
                .Include(ma => ma.Movie)
                .Include(ma => ma.Actor)
                .ToListAsync();

            var movies = await _context.Movie.OrderBy(m => m.Title).ToListAsync();
            var actors = await _context.Actor.OrderBy(a => a.Name).ToListAsync();

            ViewBag.Movies = movies;
            ViewBag.Actors = actors;
            ViewBag.MovieActors = movieActors;

            return View();
        }

        // POST: Add Actor to Movie
        [HttpPost]
        public async Task<IActionResult> AddActorToMovie(int movieId, int actorId)
        {
            try
            {
                // Check if the relationship already exists
                var existingRelationship = await _context.MovieActor
                    .FirstOrDefaultAsync(ma => ma.MovieId == movieId && ma.ActorId == actorId);

                if (existingRelationship != null)
                {
                    TempData["Error"] = "This actor is already associated with this movie.";
                    return RedirectToAction(nameof(Index));
                }

                // Verify movie and actor exist
                var movie = await _context.Movie.FindAsync(movieId);
                var actor = await _context.Actor.FindAsync(actorId);

                if (movie == null || actor == null)
                {
                    TempData["Error"] = "Invalid movie or actor selected.";
                    return RedirectToAction(nameof(Index));
                }

                // Create the relationship
                var movieActor = new MovieActor
                {
                    MovieId = movieId,
                    ActorId = actorId
                };

                _context.MovieActor.Add(movieActor);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Successfully added {actor.Name} to {movie.Title}!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error adding relationship: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Remove Actor from Movie
        [HttpPost]
        public async Task<IActionResult> RemoveActorFromMovie(int movieId, int actorId)
        {
            try
            {
                var movieActor = await _context.MovieActor
                    .FirstOrDefaultAsync(ma => ma.MovieId == movieId && ma.ActorId == actorId);

                if (movieActor == null)
                {
                    TempData["Error"] = "Relationship not found.";
                    return RedirectToAction(nameof(Index));
                }

                _context.MovieActor.Remove(movieActor);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Successfully removed actor from movie!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error removing relationship: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}