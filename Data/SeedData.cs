using MvcMovie.Models;

namespace MvcMovie.Data;

public static class SeedData
{
    public static void Initialize(MvcMovieContext context)
    {
        if (context.Movies.Any())
        {
            return;
        }

        var releaseDate = new DateTime(1986, 10, 1);

        context.Movies.AddRange(
            new Movie
            {
                Title = "El Conde de Montecristo",
                ReleaseDate = releaseDate,
                Genre = "Adventure",
                Price = 999m,
                Rating = "PG-13"
            },
            new Movie
            {
                Title = "Pirates of the Caribbean: The Curse of the Black Pearl",
                ReleaseDate = releaseDate,
                Genre = "Action-Adventure",
                Price = 10m,
                Rating = "PG-13"
            },
            new Movie
            {
                Title = "Ratatouille",
                ReleaseDate = releaseDate,
                Genre = "Animation",
                Price = 8m,
                Rating = "G"
            });

        context.SaveChanges();
    }
}