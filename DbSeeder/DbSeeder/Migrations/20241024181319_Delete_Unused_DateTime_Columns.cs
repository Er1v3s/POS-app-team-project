using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbSeeder.Migrations
{
    /// <inheritdoc />
    public partial class Delete_Unused_DateTime_Columns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentTime",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "OriderTime",
                table: "OrderItems");

            migrationBuilder.RenameColumn(
                name: "OrdersOrderId",
                table: "OrderItems",
                newName: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "OrderItems",
                newName: "OrdersOrderId");

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentTime",
                table: "Payments",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "OriderTime",
                table: "OrderItems",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
