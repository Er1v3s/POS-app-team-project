using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POS.Migrations
{
    /// <inheritdoc />
    public partial class SecondStageOfProjectMigration_fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "creationDate",
                table: "ToDoListTasks",
                newName: "CreationDate");

            migrationBuilder.RenameColumn(
                name: "content",
                table: "ToDoListTasks",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "completionDate",
                table: "ToDoListTasks",
                newName: "CompletionDate");

            migrationBuilder.RenameColumn(
                name: "todoTask_Id",
                table: "ToDoListTasks",
                newName: "TodoTask_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreationDate",
                table: "ToDoListTasks",
                newName: "creationDate");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "ToDoListTasks",
                newName: "content");

            migrationBuilder.RenameColumn(
                name: "CompletionDate",
                table: "ToDoListTasks",
                newName: "completionDate");

            migrationBuilder.RenameColumn(
                name: "TodoTask_Id",
                table: "ToDoListTasks",
                newName: "todoTask_Id");
        }
    }
}
