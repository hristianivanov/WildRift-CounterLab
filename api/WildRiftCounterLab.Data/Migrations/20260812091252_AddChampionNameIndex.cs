using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WildRiftCounterLab.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddChampionNameIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Champions_Name",
                table: "Champions",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Champions_Name",
                table: "Champions");
        }
    }
}
