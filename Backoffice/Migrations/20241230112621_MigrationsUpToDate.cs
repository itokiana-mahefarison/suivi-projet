using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backoffice.Migrations
{
    /// <inheritdoc />
    public partial class MigrationsUpToDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskLink_Tasks_TaskFromId",
                table: "TaskLink");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskLink_Tasks_TaskToId",
                table: "TaskLink");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskLink",
                table: "TaskLink");

            migrationBuilder.RenameTable(
                name: "TaskLink",
                newName: "TaskLinks");

            migrationBuilder.RenameIndex(
                name: "IX_TaskLink_TaskToId",
                table: "TaskLinks",
                newName: "IX_TaskLinks_TaskToId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskLinks",
                table: "TaskLinks",
                columns: new[] { "TaskFromId", "TaskToId" });

            migrationBuilder.AddForeignKey(
                name: "FK_TaskLinks_Tasks_TaskFromId",
                table: "TaskLinks",
                column: "TaskFromId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskLinks_Tasks_TaskToId",
                table: "TaskLinks",
                column: "TaskToId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskLinks_Tasks_TaskFromId",
                table: "TaskLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskLinks_Tasks_TaskToId",
                table: "TaskLinks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskLinks",
                table: "TaskLinks");

            migrationBuilder.RenameTable(
                name: "TaskLinks",
                newName: "TaskLink");

            migrationBuilder.RenameIndex(
                name: "IX_TaskLinks_TaskToId",
                table: "TaskLink",
                newName: "IX_TaskLink_TaskToId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskLink",
                table: "TaskLink",
                columns: new[] { "TaskFromId", "TaskToId" });

            migrationBuilder.AddForeignKey(
                name: "FK_TaskLink_Tasks_TaskFromId",
                table: "TaskLink",
                column: "TaskFromId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskLink_Tasks_TaskToId",
                table: "TaskLink",
                column: "TaskToId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
