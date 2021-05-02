using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    PlayerId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    PanelsAndShips = table.Column<string>(type: "TEXT", nullable: true),
                    StartingPanelsAndShips = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.PlayerId);
                });

            migrationBuilder.CreateTable(
                name: "SavedGames",
                columns: table => new
                {
                    SavedGameId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Height = table.Column<int>(type: "INTEGER", nullable: false),
                    Width = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayerOnePlayerId = table.Column<int>(type: "INTEGER", nullable: true),
                    PlayerTwoPlayerId = table.Column<int>(type: "INTEGER", nullable: true),
                    WinningPlayer = table.Column<string>(type: "TEXT", nullable: true),
                    Moves = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedGames", x => x.SavedGameId);
                    table.ForeignKey(
                        name: "FK_SavedGames_Players_PlayerOnePlayerId",
                        column: x => x.PlayerOnePlayerId,
                        principalTable: "Players",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SavedGames_Players_PlayerTwoPlayerId",
                        column: x => x.PlayerTwoPlayerId,
                        principalTable: "Players",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SavedGames_PlayerOnePlayerId",
                table: "SavedGames",
                column: "PlayerOnePlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedGames_PlayerTwoPlayerId",
                table: "SavedGames",
                column: "PlayerTwoPlayerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SavedGames");

            migrationBuilder.DropTable(
                name: "Players");
        }
    }
}
