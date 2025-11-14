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
    public class ActorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IActorTweetService _actorTweetService;

        public ActorsController(ApplicationDbContext context, IActorTweetService actorTweetService)
        {
            _context = context;
            _actorTweetService = actorTweetService;
        }

        // GET: Actors
        public async Task<IActionResult> Index()
        {
            return View(await _context.Actor.ToListAsync());
        }

        // GET: Actors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actor
                .Include(a => a.MovieActors)
                .ThenInclude(ma => ma.Movie)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (actor == null)
            {
                return NotFound();
            }

            return View(actor);
        }

        // GET: Actors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Actors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,IMDB_link,Gender,Age,PhotoFile")] Actor actor)
        {
            if (ModelState.IsValid)
            {
                // Handle photo file upload
                if (actor.PhotoFile != null && actor.PhotoFile.Length > 0)
                {
                    // Validate file size (max 5MB)
                    if (actor.PhotoFile.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("PhotoFile", "File size cannot exceed 5MB");
                        return View(actor);
                    }

                    // Validate file type
                    var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
                    if (!allowedTypes.Contains(actor.PhotoFile.ContentType.ToLower()))
                    {
                        ModelState.AddModelError("PhotoFile", "Only image files (JPEG, PNG, GIF, WebP) are allowed");
                        return View(actor);
                    }

                    // Convert to byte array
                    using (var memoryStream = new MemoryStream())
                    {
                        await actor.PhotoFile.CopyToAsync(memoryStream);
                        actor.Photo = memoryStream.ToArray();
                    }
                }

                _context.Add(actor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(actor);
        }

        // GET: Actors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actor.FindAsync(id);
            if (actor == null)
            {
                return NotFound();
            }
            return View(actor);
        }

        // POST: Actors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,IMDB_link,Gender,Age,ID,PhotoFile")] Actor actor)
        {
            if (id != actor.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Get the existing actor to preserve the current photo if no new one is uploaded
                    var existingActor = await _context.Actor.AsNoTracking().FirstOrDefaultAsync(m => m.ID == id);
                    if (existingActor == null)
                    {
                        return NotFound();
                    }

                    // Handle photo file upload
                    if (actor.PhotoFile != null && actor.PhotoFile.Length > 0)
                    {
                        // Validate file size (max 5MB)
                        if (actor.PhotoFile.Length > 5 * 1024 * 1024)
                        {
                            ModelState.AddModelError("PhotoFile", "File size cannot exceed 5MB");
                            return View(actor);
                        }

                        // Validate file type
                        var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
                        if (!allowedTypes.Contains(actor.PhotoFile.ContentType.ToLower()))
                        {
                            ModelState.AddModelError("PhotoFile", "Only image files (JPEG, PNG, GIF, WebP) are allowed");
                            return View(actor);
                        }

                        // Convert to byte array
                        using (var memoryStream = new MemoryStream())
                        {
                            await actor.PhotoFile.CopyToAsync(memoryStream);
                            actor.Photo = memoryStream.ToArray();
                        }
                    }
                    else
                    {
                        // Keep the existing photo if no new file is uploaded
                        actor.Photo = existingActor.Photo;
                    }

                    _context.Update(actor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActorExists(actor.ID))
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
            return View(actor);
        }

        // GET: Actors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actor
                .FirstOrDefaultAsync(m => m.ID == id);
            if (actor == null)
            {
                return NotFound();
            }

            return View(actor);
        }

        // POST: Actors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var actor = await _context.Actor.FindAsync(id);
            if (actor != null)
            {
                _context.Actor.Remove(actor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Get actor photo
        public async Task<IActionResult> GetActorPhoto(int id)
        {
            var actor = await _context.Actor.FindAsync(id);
            if (actor?.Photo == null)
            {
                return NotFound();
            }

            return File(actor.Photo, "image/jpeg"); // Default to JPEG content type
        }

        // Generate AI Tweets for an actor
        [HttpPost]
        public async Task<IActionResult> GenerateTweets(int id)
        {
            try
            {
                var actor = await _context.Actor
                    .Include(a => a.MovieActors)
                    .ThenInclude(ma => ma.Movie)
                    .FirstOrDefaultAsync(a => a.ID == id);

                if (actor == null)
                {
                    return Json(new { success = false, error = "Actor not found" });
                }

                var movieTitles = actor.MovieActors.Select(ma => ma.Movie.Title).ToList();
                var tweetsResult = await _actorTweetService.GenerateActorTweetsAsync(actor, movieTitles);

                return Json(new { success = true, data = tweetsResult });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        private bool ActorExists(int id)
        {
            return _context.Actor.Any(e => e.ID == id);
        }
    }
}
