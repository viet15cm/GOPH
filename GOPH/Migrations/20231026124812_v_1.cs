using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GOPH.Migrations
{
    /// <inheritdoc />
    public partial class v_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Products_Id",
                table: "Products",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommodityGroups_Id",
                table: "CommodityGroups",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Commodities_Id",
                table: "Commodities",
                column: "Id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_Id",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_CommodityGroups_Id",
                table: "CommodityGroups");

            migrationBuilder.DropIndex(
                name: "IX_Commodities_Id",
                table: "Commodities");

            migrationBuilder.DropColumn(
                name: "Rank",
                table: "Products");
        }
    }
}
