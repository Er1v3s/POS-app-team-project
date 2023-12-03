using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POS.Migrations
{
    /// <inheritdoc />
    public partial class Order : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    OrderItem_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrdersOrder_id = table.Column<int>(type: "INTEGER", nullable: false),
                    Employee_id = table.Column<int>(type: "INTEGER", nullable: false),
                    Product_id = table.Column<int>(type: "INTEGER", nullable: false),
                    Orider_time = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.OrderItem_id);
                    table.ForeignKey(
                      name: "FK_OrderItems_Orders_OrdersOrder_id",
                      column: x => x.OrdersOrder_id,
                      principalTable: "Orders",
                      principalColumn: "Order_id");
                });

            migrationBuilder.AddColumn<int>(
                name: "OrdersOrder_id",
                table: "Payments",
                type: "INTEGER",
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrdersOrder_id",
                table: "Payments",
                column: "OrdersOrder_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Orders_OrdersOrder_id",
                table: "Payments",
                column: "OrdersOrder_id",
                principalTable: "Orders",
                principalColumn: "Order_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.DropColumn("Orider_time", "Orders");
            migrationBuilder.DropColumn("Quantity", "Orders");
            migrationBuilder.DropColumn("Order_id", "Payments");

            migrationBuilder.CreateTable(
                name: "EmployeeWorkSession",
                columns: table => new
                {
                    Work_Session_Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Employee_Id = table.Column<int>(type: "INTEGER", nullable: true),
                    Employee_Name = table.Column<string>(type: "TEXT", nullable: true),
                    Working_Time_From = table.Column<string>(type: "TEXT", nullable: true),
                    Working_Time_To = table.Column<string>(type: "TEXT", nullable: true),
                    Working_Time_Summary = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeWorkSession", x => x.Work_Session_Id);
                });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropTable(name: "OrderItems");   

            migrationBuilder.DropTable(name: "EmployeeWorkSession");
            migrationBuilder.DropColumn("OrdersOrder_Id", "Payments");

            migrationBuilder.AddColumn<int>(
                name: "Order_id",
                table: "Payments",
                type: "INTEGER",
                nullable: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "Orider_time",
                table: "Orders",
                type: "TEXT",
                nullable: false);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Orders",
                type: "INTEGER",
                nullable: false);
        }
    }
}
