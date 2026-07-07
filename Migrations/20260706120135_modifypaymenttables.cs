using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ostawy.Migrations
{
    /// <inheritdoc />
    public partial class modifypaymenttables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StripePriceId",
                table: "Plans");

            migrationBuilder.RenameColumn(
                name: "StripeSessionId",
                table: "Payments",
                newName: "PaymobTransactionId");

            migrationBuilder.RenameColumn(
                name: "StripePaymentIntentId",
                table: "Payments",
                newName: "PaymobOrderId");

            migrationBuilder.AddColumn<string>(
                name: "LatestPaymobOrderId",
                table: "UserSubscriptions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LatestPaymobOrderId",
                table: "UserSubscriptions");

            migrationBuilder.RenameColumn(
                name: "PaymobTransactionId",
                table: "Payments",
                newName: "StripeSessionId");

            migrationBuilder.RenameColumn(
                name: "PaymobOrderId",
                table: "Payments",
                newName: "StripePaymentIntentId");

            migrationBuilder.AddColumn<string>(
                name: "StripePriceId",
                table: "Plans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
