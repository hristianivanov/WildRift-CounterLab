using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WildRiftCounterLab.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMatchupTips : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MatchupTips",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Champion = table.Column<string>(type: "text", nullable: false),
                    EnemyChampion = table.Column<string>(type: "text", nullable: false),
                    Tip = table.Column<string>(type: "text", nullable: false),
                    AbilityTag = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchupTips", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MatchupTips_Champion_EnemyChampion",
                table: "MatchupTips",
                columns: new[] { "Champion", "EnemyChampion" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MatchupTips");
        }
    }
}
