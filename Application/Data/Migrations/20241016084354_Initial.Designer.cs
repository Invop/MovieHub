﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MovieHub.Application.Data;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MovieHub.Application.Data.Migrations
{
    [DbContext(typeof(MovieHubDbContext))]
    [Migration("20241016084354_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("MovieHub.Application.Models.Genre", b =>
                {
                    b.Property<Guid>("MovieId")
                        .HasColumnType("uuid")
                        .HasColumnName("movieid");

                    b.Property<int>("GenreId")
                        .HasColumnType("integer")
                        .HasColumnName("genreid");

                    b.HasKey("MovieId", "GenreId");

                    b.HasIndex("GenreId");

                    b.ToTable("movie_genres", (string)null);
                });

            modelBuilder.Entity("MovieHub.Application.Models.GenreLookup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("name");

                    b.HasKey("Id");

                    b.ToTable("genres", (string)null);
                });

            modelBuilder.Entity("MovieHub.Application.Models.Movie", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Overview")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)")
                        .HasColumnName("overview");

                    b.Property<string>("PosterBase64")
                        .HasColumnType("text")
                        .HasColumnName("poster_base64");

                    b.Property<float?>("Rating")
                        .HasColumnType("real");

                    b.Property<string>("Slug")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("slug");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("title");

                    b.Property<int?>("UserRating")
                        .HasColumnType("integer");

                    b.Property<int>("YearOfRelease")
                        .HasColumnType("integer")
                        .HasColumnName("yearofrelease");

                    b.HasKey("Id");

                    b.HasIndex("Slug")
                        .IsUnique()
                        .HasDatabaseName("movies_slug_idx");

                    b.ToTable("movies", (string)null);
                });

            modelBuilder.Entity("MovieHub.Application.Models.MovieRating", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("userid");

                    b.Property<Guid>("MovieId")
                        .HasColumnType("uuid")
                        .HasColumnName("movieid");

                    b.Property<int>("Rating")
                        .HasColumnType("integer")
                        .HasColumnName("rating");

                    b.HasKey("UserId", "MovieId");

                    b.HasIndex("MovieId");

                    b.ToTable("ratings", (string)null);
                });

            modelBuilder.Entity("MovieHub.Application.Models.Genre", b =>
                {
                    b.HasOne("MovieHub.Application.Models.GenreLookup", "GenreLookup")
                        .WithMany("Genres")
                        .HasForeignKey("GenreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MovieHub.Application.Models.Movie", "Movie")
                        .WithMany("Genres")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GenreLookup");

                    b.Navigation("Movie");
                });

            modelBuilder.Entity("MovieHub.Application.Models.MovieRating", b =>
                {
                    b.HasOne("MovieHub.Application.Models.Movie", "Movie")
                        .WithMany("Ratings")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Movie");
                });

            modelBuilder.Entity("MovieHub.Application.Models.GenreLookup", b =>
                {
                    b.Navigation("Genres");
                });

            modelBuilder.Entity("MovieHub.Application.Models.Movie", b =>
                {
                    b.Navigation("Genres");

                    b.Navigation("Ratings");
                });
#pragma warning restore 612, 618
        }
    }
}