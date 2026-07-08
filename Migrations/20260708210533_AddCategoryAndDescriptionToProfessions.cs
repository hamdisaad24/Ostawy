using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ostawy.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryAndDescriptionToProfessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add CategoryId and Description columns to Professions table.
            // Use default values for existing rows to keep the migration safe on existing data.
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Professions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Professions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: string.Empty);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Professions");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Professions");
        }
    }
}
