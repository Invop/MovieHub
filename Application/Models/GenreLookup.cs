using System.ComponentModel.DataAnnotations;

namespace MovieHub.Application.Models;

public class GenreLookup
{
    private string _name;
    public int Id { get; init; }

    [Required]
    [StringLength(30)]
    public string Name
    {
        get => _name;
        set => _name = NormalizeName(value);
    }

    public ICollection<Genre> Genres { get; set; } = new List<Genre>();

    private static string NormalizeName(string name)
    {
        return name.Trim().ToLower();
    }
}