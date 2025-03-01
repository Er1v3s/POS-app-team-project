using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbSeeder.Migrations
{
    /// <inheritdoc />
    public partial class Set_DayOfTheWeek_InOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DayOfWeek",
                table: "Orders",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DayOfWeek",
                table: "Orders");
        }
    }
}
