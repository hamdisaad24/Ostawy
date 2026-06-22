using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ostawy.Migrations
{
    /// <inheritdoc />
    public partial class SeedWorkersData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Category", "Email", "FullName", "IsAvailable", "Lat", "Lng", "PasswordHash", "Price", "Rating", "ReviewsCount", "Role", "Specialty" },
                values: new object[,]
                {
                    { 1, "plumbing", "ahmed@ostawy.com", "أحمد محمد", true, 30.0444, 31.235700000000001, "123456", 80m, 4.9000000000000004, 128, "worker", "سباكة" },
                    { 2, "electric", "mahmoud@ostawy.com", "محمود خالد", true, 30.059999999999999, 31.260000000000002, "123456", 100m, 4.7999999999999998, 97, "worker", "كهرباء" },
                    { 3, "ac", "samer@ostawy.com", "سامر علي", true, 30.035, 31.219999999999999, "123456", 150m, 4.7000000000000002, 64, "worker", "تكييف وتبريد" },
                    { 4, "paint", "youssef_h@ostawy.com", "يوسف حسن", true, 30.07, 31.280000000000001, "123456", 70m, 4.9000000000000004, 211, "worker", "دهانات" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
