using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GOPH.Migrations
{
    /// <inheritdoc />
    public partial class v_16 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rank",
                table: "Products");

            migrationBuilder.AddColumn<bool>(
                name: "IsEvent",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEvent",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
