using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ostawy.Migrations
{
    /// <inheritdoc />
    public partial class AddNumberOfTryToEmailVerificationsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfTry",
                table: "EmailVerifications",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfTry",
                table: "EmailVerifications");
        }
    }
}
