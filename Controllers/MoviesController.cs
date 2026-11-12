using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcMovie.Data;
using MvcMovie.Models;

namespace MvcMovie.Controllers;

public class MoviesController : Controller
{
    private readonly MvcMovieContext _context;

    public MoviesController(MvcMovieContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? title, string? genre, int? fromYear)
    {
        var genres = await _context.Movies
            .Select(movie => movie.Genre)
            .Distinct()
            .OrderBy(value => value)
            .ToListAsync();

        ViewData["SearchTitle"] = title;
        ViewData["Genres"] = new SelectList(genres, genre);
        ViewData["FromYear"] = fromYear;

        var movies = _context.Movies.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(title))
        {
            var searchTitle = title.Trim().ToLowerInvariant();
            movies = movies.Where(movie => movie.Title.ToLower().Contains(searchTitle));
        }

        if (!string.IsNullOrWhiteSpace(genre))
        {
            movies = movies.Where(movie => movie.Genre == genre);
        }

        if (fromYear is >= 1 and <= 9999)
        {
            var startDate = new DateTime(fromYear.Value, 1, 1);
            movies = movies.Where(movie => movie.ReleaseDate >= startDate);
        }

        return View(await movies.OrderBy(movie => movie.Title).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var movie = await _context.Movies.FindAsync(id);
        return movie is null ? NotFound() : View(movie);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("Title,ReleaseDate,Genre,Price,Rating")] Movie movie)
    {
        if (!ModelState.IsValid)
        {
            return View(movie);
        }

        _context.Movies.Add(movie);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var movie = await _context.Movies.FindAsync(id);
        return movie is null ? NotFound() : View(movie);
    }

    [HttpPost, ActionName("Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditConfirmed(int id)
    {
        var movie = await _context.Movies.FindAsync(id);
        if (movie is null)
        {
            return NotFound();
        }

        var updated = await TryUpdateModelAsync(
            movie,
            string.Empty,
            value => value.Title,
            value => value.ReleaseDate,
            value => value.Genre,
            value => value.Price,
            value => value.Rating);

        if (!updated)
        {
            return View(movie);
        }

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Movies.AnyAsync(value => value.Id == id))
            {
                return NotFound();
            }

            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var movie = await _context.Movies.FindAsync(id);
        return movie is null ? NotFound() : View(movie);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var movie = await _context.Movies.FindAsync(id);
        if (movie is not null)
        {
            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}