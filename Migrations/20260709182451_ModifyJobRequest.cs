using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ostawy.Migrations
{
    /// <inheritdoc />
    public partial class ModifyJobRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "JobRequests");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "JobRequests",
                newName: "UserId");

            migrationBuilder.AddColumn<Guid>(
                name: "ProfessionId",
                table: "JobRequests",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_JobRequests_ProfessionId",
                table: "JobRequests",
                column: "ProfessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobRequests_Professions_ProfessionId",
                table: "JobRequests",
                column: "ProfessionId",
                principalTable: "Professions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobRequests_Professions_ProfessionId",
                table: "JobRequests");

            migrationBuilder.DropIndex(
                name: "IX_JobRequests_ProfessionId",
                table: "JobRequests");

            migrationBuilder.DropColumn(
                name: "ProfessionId",
                table: "JobRequests");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "JobRequests",
                newName: "CategoryId");

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "JobRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
