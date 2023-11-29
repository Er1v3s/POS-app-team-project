using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POS.Migrations
{
    /// <inheritdoc />
    public partial class IsUserLoggedInMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Ingredient_id1",
                table: "RecipeIngredients",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Recipe_id1",
                table: "RecipeIngredients",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Is_User_LoggedIn",
                table: "Employees",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_RecipeIngredients_Ingredient_id1",
                table: "RecipeIngredients",
                column: "Ingredient_id1");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeIngredients_Recipe_id1",
                table: "RecipeIngredients",
                column: "Recipe_id1");

            migrationBuilder.AddForeignKey(
                name: "FK_RecipeIngredients_Ingredients_Ingredient_id1",
                table: "RecipeIngredients",
                column: "Ingredient_id1",
                principalTable: "Ingredients",
                principalColumn: "Ingredient_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RecipeIngredients_Recipes_Recipe_id1",
                table: "RecipeIngredients",
                column: "Recipe_id1",
                principalTable: "Recipes",
                principalColumn: "Recipe_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecipeIngredients_Ingredients_Ingredient_id1",
                table: "RecipeIngredients");

            migrationBuilder.DropForeignKey(
                name: "FK_RecipeIngredients_Recipes_Recipe_id1",
                table: "RecipeIngredients");

            migrationBuilder.DropIndex(
                name: "IX_RecipeIngredients_Ingredient_id1",
                table: "RecipeIngredients");

            migrationBuilder.DropIndex(
                name: "IX_RecipeIngredients_Recipe_id1",
                table: "RecipeIngredients");

            migrationBuilder.DropColumn(
                name: "Ingredient_id1",
                table: "RecipeIngredients");

            migrationBuilder.DropColumn(
                name: "Recipe_id1",
                table: "RecipeIngredients");

            migrationBuilder.DropColumn(
                name: "Is_User_LoggedIn",
                table: "Employees");
        }
    }
}
