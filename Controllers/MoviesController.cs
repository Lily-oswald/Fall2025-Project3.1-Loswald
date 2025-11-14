using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Fall2025_Project3._1_Loswald.Data;
using Fall2025_Project3._1_Loswald.Models;
using Fall2025_Project3._1_Loswald.Services;

namespace Fall2025_Project3._1_Loswald.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMovieReviewService _movieReviewService;

        public MoviesController(ApplicationDbContext context, IMovieReviewService movieReviewService)
        {
            _context = context;
            _movieReviewService = movieReviewService;
        }

        // GET: Movies
        public async Task<IActionResult> Index()
        {
            return View(await _context.Movie.ToListAsync());
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .Include(m => m.MovieActors)
                .ThenInclude(ma => ma.Actor)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,IMDB_link,genre,ReleaseYear,PosterFile")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                // Handle poster file upload
                if (movie.PosterFile != null && movie.PosterFile.Length > 0)
                {
                    // Validate file size (max 5MB)
                    if (movie.PosterFile.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("PosterFile", "File size cannot exceed 5MB");
                        return View(movie);
                    }

                    // Validate file type
                    var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
                    if (!allowedTypes.Contains(movie.PosterFile.ContentType.ToLower()))
                    {
                        ModelState.AddModelError("PosterFile", "Only image files (JPEG, PNG, GIF, WebP) are allowed");
                        return View(movie);
                    }

                    // Convert to byte array
                    using (var memoryStream = new MemoryStream())
                    {
                        await movie.PosterFile.CopyToAsync(memoryStream);
                        movie.Poster = memoryStream.ToArray();
                    }
                }

                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // POST: Movies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Title,IMDB_link,genre,ReleaseYear,ID,PosterFile")] Movie movie)
        {
            if (id != movie.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Get the existing movie to preserve the current poster if no new one is uploaded
                    var existingMovie = await _context.Movie.AsNoTracking().FirstOrDefaultAsync(m => m.ID == id);
                    if (existingMovie == null)
                    {
                        return NotFound();
                    }

                    // Handle poster file upload
                    if (movie.PosterFile != null && movie.PosterFile.Length > 0)
                    {
                        // Validate file size (max 5MB)
                        if (movie.PosterFile.Length > 5 * 1024 * 1024)
                        {
                            ModelState.AddModelError("PosterFile", "File size cannot exceed 5MB");
                            return View(movie);
                        }

                        // Validate file type
                        var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
                        if (!allowedTypes.Contains(movie.PosterFile.ContentType.ToLower()))
                        {
                            ModelState.AddModelError("PosterFile", "Only image files (JPEG, PNG, GIF, WebP) are allowed");
                            return View(movie);
                        }

                        // Convert to byte array
                        using (var memoryStream = new MemoryStream())
                        {
                            await movie.PosterFile.CopyToAsync(memoryStream);
                            movie.Poster = memoryStream.ToArray();
                        }
                    }
                    else
                    {
                        // Keep the existing poster if no new file is uploaded
                        movie.Poster = existingMovie.Poster;
                    }

                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.ID == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movie.FindAsync(id);
            if (movie != null)
            {
                _context.Movie.Remove(movie);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Get poster image
        public async Task<IActionResult> GetPosterImage(int id)
        {
            var movie = await _context.Movie.FindAsync(id);
            if (movie?.Poster == null)
            {
                return NotFound();
            }

            return File(movie.Poster, "image/jpeg"); // Default to JPEG content type
        }

        // Generate AI Reviews for a movie
        [HttpPost]
        public async Task<IActionResult> GenerateReviews(int id)
        {
            try
            {
                var movie = await _context.Movie
                    .Include(m => m.MovieActors)
                    .ThenInclude(ma => ma.Actor)
                    .FirstOrDefaultAsync(m => m.ID == id);

                if (movie == null)
                {
                    return Json(new { success = false, error = "Movie not found" });
                }

                var actors = movie.MovieActors.Select(ma => ma.Actor).ToList();
                var reviewsResult = await _movieReviewService.GenerateMovieReviewsAsync(movie, actors);

                return Json(new { success = true, data = reviewsResult });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        private bool MovieExists(int id)
        {
            return _context.Movie.Any(e => e.ID == id);
        }
    }
}
