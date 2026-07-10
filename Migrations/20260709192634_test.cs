using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ostawy.Migrations
{
    /// <inheritdoc />
    public partial class test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_JobRequests_UserId",
                table: "JobRequests",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobRequests_AspNetUsers_UserId",
                table: "JobRequests",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobRequests_AspNetUsers_UserId",
                table: "JobRequests");

            migrationBuilder.DropIndex(
                name: "IX_JobRequests_UserId",
                table: "JobRequests");
        }
    }
}
