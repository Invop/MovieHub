using Microsoft.EntityFrameworkCore;
using MovieHub.Application.Data;
using MovieHub.Application.Models;

namespace MovieHub.Application.Infrastructure.Repositories;

public class GenreRepository : IGenreRepository
{
    private readonly MovieHubDbContext _context;

    public GenreRepository(MovieHubDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<GenreLookup>> GetAllAsync(CancellationToken token = default)
    {
        return await _context.GenreLookups.ToListAsync(token);
    }

    public async Task<GenreLookup?> GetByIdAsync(int id, CancellationToken token = default)
    {
        return await _context.GenreLookups.FindAsync([id], token);
    }

    public async Task<GenreLookup?> GetByNameAsync(string name, CancellationToken token)
    {
        return await _context.Set<GenreLookup>()
            .FirstOrDefaultAsync(g => g.Name == name, token);
    }

    public async Task<bool> CreateAsync(GenreLookup genre, CancellationToken token = default)
    {
        _context.GenreLookups.Add(genre);
        var result = await _context.SaveChangesAsync(token);
        return result > 0;
    }

    public async Task<bool> UpdateAsync(GenreLookup genre, CancellationToken token = default)
    {
        var existingGenre = await _context.GenreLookups
            .Include(gl => gl.Genres)
            .FirstOrDefaultAsync(gl => gl.Id == genre.Id, token);

        if (existingGenre == null) return false;

        // Update the GenreLookup fields
        existingGenre.Name = genre.Name;

        // Update related Genre entities if necessary
        foreach (var genreEntity in existingGenre.Genres)
            // Assuming the logic to update related genres is contained here.
            // For example, if you want to update a field in Genre
            genreEntity.MovieId = genreEntity.MovieId; // Example modification

        var result = await _context.SaveChangesAsync(token);
        return result > 0;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken token = default)
    {
        var genre = await _context.GenreLookups.FindAsync([id], token);
        if (genre == null) return false;

        _context.GenreLookups.Remove(genre);
        var result = await _context.SaveChangesAsync(token);
        return result > 0;
    }
}