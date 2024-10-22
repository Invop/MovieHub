using System.Text.RegularExpressions;
using Bogus;

namespace MovieHub.Application.Models;

public partial class Movie
{
    private static readonly Faker Faker = new();
    private string? _slug;
    private string _title = default!;
    private int _yearOfRelease;
    public Guid Id { get; set; }

    public string Slug
    {
        get => _slug ??= GenerateSlug();
        set => _slug = value;
    }

    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            _slug = null; // Reset slug when title changes
        }
    }

    public int YearOfRelease
    {
        get => _yearOfRelease;
        set
        {
            _yearOfRelease = value;
            _slug = null; // Reset slug when year changes
        }
    }

    public float? Rating { get; set; }
    public int? UserRating { get; set; }

    public string? PosterBase64 { get; set; } = string.Empty;

    public string? Overview { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<Genre> Genres { get; set; } = new List<Genre>();
    public ICollection<MovieRating> Ratings { get; set; } = new List<MovieRating>();

    private string GenerateSlug()
    {
        var sluggedTitle = SlugRegex().Replace(Title, string.Empty)
            .ToLower().Replace(" ", "-");
        return $"{sluggedTitle}-{YearOfRelease}";
    }

    [GeneratedRegex("[^0-9A-Za-z _-]", RegexOptions.NonBacktracking, 5)]
    private static partial Regex SlugRegex();
}