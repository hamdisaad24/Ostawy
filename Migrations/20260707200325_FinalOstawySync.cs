using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ostawy.Migrations
{
    /// <inheritdoc />
    public partial class FinalOstawySync : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddForeignKey(
            //    name: "FK_UserSubscriptions_AspNetUsers_UserId",
            //    table: "UserSubscriptions",
            //    column: "UserId",
            //    principalTable: "AspNetUsers",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Cascade);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_UserSubscriptions_Plans_PlanId",
            //    table: "UserSubscriptions",
            //    column: "PlanId",
            //    principalTable: "Plans",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_UserSubscriptions_AspNetUsers_UserId",
            //    table: "UserSubscriptions");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_UserSubscriptions_Plans_PlanId",
            //    table: "UserSubscriptions");
        }
    }
}
