using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieHub.API.Migrations
{
    /// <inheritdoc />
    public partial class AddedTableMovieReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MovieReview",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    movieId = table.Column<int>(type: "INTEGER", nullable: false),
                    score = table.Column<decimal>(type: "TEXT", precision: 4, scale: 2, nullable: false),
                    comment = table.Column<string>(type: "TEXT", nullable: true),
                    reviewDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieReview", x => x.id);
                    table.ForeignKey(
                        name: "FK_MovieReview_Movie_movieId",
                        column: x => x.movieId,
                        principalTable: "Movie",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovieReview_movieId",
                table: "MovieReview",
                column: "movieId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovieReview");
        }
    }
}
