using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GOPH.Migrations
{
    /// <inheritdoc />
    public partial class v_22 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalPrice",
                table: "Invoices",
                newName: "TotalPirce");

            migrationBuilder.AddColumn<bool>(
                name: "IsCloseTheOrder",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCloseTheOrder",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "TotalPirce",
                table: "Invoices",
                newName: "TotalPrice");
        }
    }
}
