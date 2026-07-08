using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ostawy.Migrations
{
    /// <inheritdoc />
    public partial class SyncDatabaseSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "Specialty",
            //    table: "AspNetUsers");

            //migrationBuilder.AddColumn<int>(
            //    name: "CategoryId",
            //    table: "Craftsmen",
            //    type: "int",
            //    nullable: false,
            //    defaultValue: 0);

            //migrationBuilder.CreateTable(
            //    name: "Professions",
            //    columns: table => new
            //    {
            //        Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //        CategoryId = table.Column<int>(type: "int", nullable: false),
            //        Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Professions", x => x.Id);
            //    });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        //    migrationBuilder.DropTable(
        //        name: "Professions");

        //    migrationBuilder.DropColumn(
        //        name: "CategoryId",
        //        table: "Craftsmen");

        //    migrationBuilder.AddColumn<string>(
        //        name: "Specialty",
        //        table: "AspNetUsers",
        //        type: "nvarchar(max)",
        //        nullable: false,
        //        defaultValue: "");
        }
    }
}
