using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GOPH.Migrations
{
    /// <inheritdoc />
    public partial class v_24 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalPirce",
                table: "Invoices",
                newName: "TotalPrice");

            migrationBuilder.AddColumn<int>(
                name: "Promotion",
                table: "OrderProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Promotion",
                table: "OrderProducts");

            migrationBuilder.RenameColumn(
                name: "TotalPrice",
                table: "Invoices",
                newName: "TotalPirce");
        }
    }
}
