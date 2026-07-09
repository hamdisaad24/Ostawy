using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ostawy.Migrations
{
    /// <inheritdoc />
    public partial class ModifyCraftManProfessionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bio",
                table: "Craftsmen");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "Craftsmen");

            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "Craftsmen");

            migrationBuilder.DropColumn(
                name: "YearsOfExperience",
                table: "Craftsmen");

            migrationBuilder.AddColumn<string>(
                name: "Bio",
                table: "CraftManProfessions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "CraftManProfessions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "CraftManProfessions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "YearsOfExperience",
                table: "CraftManProfessions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bio",
                table: "CraftManProfessions");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "CraftManProfessions");

            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "CraftManProfessions");

            migrationBuilder.DropColumn(
                name: "YearsOfExperience",
                table: "CraftManProfessions");

            migrationBuilder.AddColumn<string>(
                name: "Bio",
                table: "Craftsmen",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "Craftsmen",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "Craftsmen",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "YearsOfExperience",
                table: "Craftsmen",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
