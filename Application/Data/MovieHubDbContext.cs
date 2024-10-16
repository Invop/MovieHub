using Microsoft.EntityFrameworkCore;
using MovieHub.Application.Models;

namespace MovieHub.Application.Data;

public class MovieHubDbContext : DbContext
{
    public MovieHubDbContext(DbContextOptions<MovieHubDbContext> options) : base(options)
    {
    }

    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<Genre> MovieGenres => Set<Genre>();
    public DbSet<GenreLookup> GenreLookups => Set<GenreLookup>();
    public DbSet<GenreLookup> Genres => Set<GenreLookup>();
    public DbSet<MovieRating> Ratings => Set<MovieRating>();

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Movie>(entity =>
    {
        entity.ToTable("movies");
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id).HasColumnName("id");
        entity.Property(e => e.Slug).HasColumnName("slug").IsRequired();
        entity.Property(e => e.Title).HasColumnName("title").IsRequired();
        entity.Property(e => e.YearOfRelease).HasColumnName("yearofrelease").IsRequired();
        entity.Property(e => e.Overview).HasColumnName("overview").HasMaxLength(500);
        entity.Property(e => e.PosterBase64).HasColumnName("poster_base64");
        entity.HasIndex(e => e.Slug)
            .IsUnique()
            .HasDatabaseName("movies_slug_idx");
    });
    
    modelBuilder.Entity<GenreLookup>(entity =>
    {
        entity.ToTable("genres");
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id).HasColumnName("id");
        entity.Property(e => e.Name).HasColumnName("name").IsRequired();
    });
    modelBuilder.Entity<Genre>(entity =>
    {
        entity.ToTable("movie_genres");
        entity.HasKey(e => new { e.MovieId, e.GenreId });

        entity.Property(e => e.MovieId).HasColumnName("movieid");
        entity.Property(e => e.GenreId).HasColumnName("genreid");

        entity.HasOne(e => e.Movie)
            .WithMany(m => m.Genres)
            .HasForeignKey(e => e.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(e => e.GenreLookup)
            .WithMany(g => g.Genres)
            .HasForeignKey(e => e.GenreId)
            .OnDelete(DeleteBehavior.Cascade);
    });
    modelBuilder.Entity<MovieRating>(entity =>
    {
        entity.ToTable("ratings");
        entity.HasKey(e => new { e.UserId, e.MovieId });

        entity.Property(e => e.UserId).HasColumnName("userid");
        entity.Property(e => e.MovieId).HasColumnName("movieid");
        entity.Property(e => e.Rating).HasColumnName("rating").IsRequired();

        entity.HasOne(e => e.Movie)
            .WithMany(m => m.Ratings)
            .HasForeignKey(e => e.MovieId)
            .OnDelete(DeleteBehavior.Cascade);
    });
}
}