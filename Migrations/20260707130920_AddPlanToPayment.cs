using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ostawy.Migrations
{
    /// <inheritdoc />
    public partial class AddPlanToPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PlanId",
                table: "Payments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PlanId",
                table: "Payments",
                column: "PlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Plans_PlanId",
                table: "Payments",
                column: "PlanId",
                principalTable: "Plans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Plans_PlanId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_PlanId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PlanId",
                table: "Payments");
        }
    }
}
