using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechSolutionsAPI.Migrations
{
    /// <inheritdoc />
    public partial class FixProvinceTypo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Province",
                table: "Addresses",
                newName: "Province");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Province",
                table: "Addresses",
                newName: "Province");
        }
    }
}
