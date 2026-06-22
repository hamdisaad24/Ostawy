using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ostawy.Migrations
{
    /// <inheritdoc />
    public partial class AddMapFieldsToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "Lat",
                table: "Users",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Lng",
                table: "Users",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Users",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<double>(
                name: "Rating",
                table: "Users",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "ReviewsCount",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Specialty",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Lat",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Lng",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ReviewsCount",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Specialty",
                table: "Users");
        }
    }
}
