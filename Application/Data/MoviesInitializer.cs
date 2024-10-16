using Microsoft.EntityFrameworkCore;
using MovieHub.Application.Models;
using Newtonsoft.Json;

namespace MovieHub.Application.Data;

public static class MoviesInitializer
{
    public static async Task Seed(MovieHubDbContext context, string filePath)
    {
        if (context.Movies.Any()) return;
        var jsonData = File.ReadAllText(filePath);
        var movies = JsonConvert.DeserializeObject<List<MovieWithGenres>>(jsonData);
        var allGenres = movies!
            .SelectMany(m => m.Genres)
            .Select(NormalizeGenre)
            .Distinct()
            .ToList();

        var genreLookup = new Dictionary<string, GenreLookup>();

        foreach (var genre in allGenres)
        {
            var genreEntity = await FindOrCreateGenreAsync(context, genre);
            genreLookup[genre] = genreEntity;
        }

        await context.SaveChangesAsync();
        var existingSlugs = new HashSet<string>(await context.Movies.Select(m => m.Slug).ToListAsync());
        foreach (var movieSeed in movies)
        {
            if (existingSlugs.Contains(movieSeed.Slug))
            {
                continue;
            }

            var movie = new Movie
            {
                Id = movieSeed.Id,
                Title = movieSeed.Title,
                YearOfRelease = movieSeed.YearOfRelease,
                Slug = movieSeed.Slug,
                PosterBase64 = movieSeed.PosterBase64,
            };
            movie.Genres = movieSeed.Genres.Select(genreName => new Genre
            {
                MovieId = movie.Id,
                GenreLookup = genreLookup[NormalizeGenre(genreName)]
            }).ToList();

            existingSlugs.Add(movieSeed.Slug);
            await context.Movies.AddAsync(movie);
        }
        await context.SaveChangesAsync();
    }

    private static async Task<GenreLookup> FindOrCreateGenreAsync(MovieHubDbContext context, string genre)
    {
        var normalizedGenre = NormalizeGenre(genre);
        var genreEntity = await context.Genres.FirstOrDefaultAsync(g => g.Name == normalizedGenre);

        if (genreEntity == null)
        {
            genreEntity = new GenreLookup { Name = normalizedGenre };
            await context.Genres.AddAsync(genreEntity);
        }

        return genreEntity;
    }
    private static string NormalizeGenre(string genre)
    {
        return genre.Trim().ToLower();
    }
    private class MovieWithGenres
    {
        public Guid Id { get; set; }
        public string Slug { get; set; }
        public string Title { get; set; }
        public int YearOfRelease { get; set; }
        public List<string> Genres { get; set; }
        public string PosterBase64 { get; set; }
    }
}