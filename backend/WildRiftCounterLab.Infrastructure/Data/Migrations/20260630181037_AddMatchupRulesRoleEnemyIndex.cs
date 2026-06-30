using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WildRiftCounterLab.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMatchupRulesRoleEnemyIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MatchupRules_Role_EnemyChampion",
                table: "MatchupRules",
                columns: new[] { "Role", "EnemyChampion" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MatchupRules_Role_EnemyChampion",
                table: "MatchupRules");
        }
    }
}
