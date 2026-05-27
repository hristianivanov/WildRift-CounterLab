using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WildRiftCounterLab.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddPlanToMatchupRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Plan",
                table: "MatchupRules",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Plan",
                table: "MatchupRules");
        }
    }
}
