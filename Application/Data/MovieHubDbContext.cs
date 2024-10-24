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

    public DbSet<Actor> Actors => Set<Actor>();
    public DbSet<MovieActor> MovieActors => Set<MovieActor>();
    public DbSet<UserList> UserLists => Set<UserList>();
    public DbSet<UserListMovie> UserListMovies => Set<UserListMovie>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Movie>(entity =>
        {
            entity.ToTable("movies");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Slug).HasColumnName("slug").HasColumnType("text").IsRequired();
            entity.Property(e => e.Title).HasColumnName("title").HasColumnType("text").IsRequired();
            entity.Property(e => e.YearOfRelease).HasColumnName("yearofrelease").IsRequired();
            entity.Property(e => e.Overview).HasColumnName("overview").HasColumnType("text");
            ;
            entity.Property(e => e.PosterBase64).HasColumnName("poster_base64").HasColumnType("text");
            ;
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
        modelBuilder.Entity<Actor>(entity =>
        {
            entity.ToTable("actors");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name").HasColumnType("text").IsRequired();
            entity.Property(e => e.Biography).HasColumnName("biography").HasColumnType("text");
            entity.Property(e => e.PhotoBase64).HasColumnName("photo_base64").HasColumnType("text");
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth").HasColumnType("timestamptz");
            entity.Property(e => e.PlaceOfBirth).HasColumnName("place_of_birth").HasColumnType("text");

            entity.HasIndex(e => e.Name)
                .HasDatabaseName("actors_name_idx");
        });
        modelBuilder.Entity<MovieActor>(entity =>
        {
            entity.ToTable("movie_actors");
            entity.HasKey(e => new { e.MovieId, e.ActorId });

            entity.Property(e => e.MovieId).HasColumnName("movie_id");
            entity.Property(e => e.ActorId).HasColumnName("actor_id");
            entity.Property(e => e.Character).HasColumnName("character").HasColumnType("text");

            entity.HasOne(e => e.Movie)
                .WithMany(m => m.MovieActors)
                .HasForeignKey(e => e.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Actor)
                .WithMany(a => a.MovieActors)
                .HasForeignKey(e => e.ActorId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserList>(entity =>
        {
            entity.ToTable("user_lists");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id").HasColumnType("text").IsRequired();
            entity.Property(e => e.Name).HasColumnName("name").HasColumnType("text").IsRequired();
            entity.Property(e => e.Description).HasColumnName("description").HasColumnType("text");
            entity.Property(e => e.IsPublic).HasColumnName("is_public");
            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at");
            
            entity.HasIndex(e => new { e.UserId, e.Name })
                .IsUnique()
                .HasDatabaseName("user_lists_userid_name_idx");
        });
        modelBuilder.Entity<UserListMovie>(entity =>
        {
            entity.ToTable("user_list_movies");
            entity.HasKey(e => new { e.ListId, e.MovieId });

            entity.Property(e => e.ListId).HasColumnName("list_id");
            entity.Property(e => e.MovieId).HasColumnName("movie_id");
            entity.Property(e => e.AddedAt)
                .HasColumnName("added_at");

            entity.HasOne(e => e.UserList)
                .WithMany(l => l.Movies)
                .HasForeignKey(e => e.ListId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Movie)
                .WithMany()
                .HasForeignKey(e => e.MovieId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}