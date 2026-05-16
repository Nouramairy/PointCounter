using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pointCounterBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddPointMatches : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PointMatches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PublicId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    HigherScoreWins = table.Column<bool>(type: "bit", nullable: false),
                    StartingScore = table.Column<int>(type: "int", nullable: false),
                    PlayersLocked = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointMatches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PointMatchPlayers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PointMatchId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    OriginalScore = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointMatchPlayers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PointMatchPlayers_PointMatches_PointMatchId",
                        column: x => x.PointMatchId,
                        principalTable: "PointMatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PointMatches_PublicId",
                table: "PointMatches",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PointMatchPlayers_PointMatchId",
                table: "PointMatchPlayers",
                column: "PointMatchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PointMatchPlayers");

            migrationBuilder.DropTable(
                name: "PointMatches");
        }
    }
}
