using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ostawy.Migrations
{
    /// <inheritdoc />
    public partial class ModifyCraftManTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "CraftManProfessions");

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "Craftsmen",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "Craftsmen");

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "CraftManProfessions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
