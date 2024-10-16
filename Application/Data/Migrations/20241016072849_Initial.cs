using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MovieHub.Application.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "genres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_genres", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "movies",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    slug = table.Column<string>(type: "text", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    yearofrelease = table.Column<int>(type: "integer", nullable: false),
                    Rating = table.Column<float>(type: "real", nullable: true),
                    UserRating = table.Column<int>(type: "integer", nullable: true),
                    poster_base64 = table.Column<string>(type: "text", nullable: true),
                    overview = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_movies", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "movie_genres",
                columns: table => new
                {
                    movieid = table.Column<Guid>(type: "uuid", nullable: false),
                    genreid = table.Column<int>(type: "integer", nullable: false),
                    GenreLookupId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_movie_genres", x => new { x.movieid, x.genreid });
                    table.ForeignKey(
                        name: "FK_movie_genres_genres_GenreLookupId",
                        column: x => x.GenreLookupId,
                        principalTable: "genres",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_movie_genres_genres_genreid",
                        column: x => x.genreid,
                        principalTable: "genres",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_movie_genres_movies_movieid",
                        column: x => x.movieid,
                        principalTable: "movies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ratings",
                columns: table => new
                {
                    userid = table.Column<Guid>(type: "uuid", nullable: false),
                    movieid = table.Column<Guid>(type: "uuid", nullable: false),
                    rating = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ratings", x => new { x.userid, x.movieid });
                    table.ForeignKey(
                        name: "FK_ratings_movies_movieid",
                        column: x => x.movieid,
                        principalTable: "movies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_movie_genres_genreid",
                table: "movie_genres",
                column: "genreid");

            migrationBuilder.CreateIndex(
                name: "IX_movie_genres_GenreLookupId",
                table: "movie_genres",
                column: "GenreLookupId");

            migrationBuilder.CreateIndex(
                name: "movies_slug_idx",
                table: "movies",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ratings_movieid",
                table: "ratings",
                column: "movieid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "movie_genres");

            migrationBuilder.DropTable(
                name: "ratings");

            migrationBuilder.DropTable(
                name: "genres");

            migrationBuilder.DropTable(
                name: "movies");
        }
    }
}
