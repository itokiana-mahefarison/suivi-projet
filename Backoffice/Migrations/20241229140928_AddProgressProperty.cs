using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backoffice.Migrations
{
    /// <inheritdoc />
    public partial class AddProgressProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Progress",
                table: "Projects",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Progress",
                table: "Projects");
        }
    }
}
