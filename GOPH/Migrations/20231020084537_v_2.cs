using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GOPH.Migrations
{
    /// <inheritdoc />
    public partial class v_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_commodidyGroups_commodidyGroups_ParentCommodityGroupId",
                table: "commodidyGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_commodidyGroups_CommodityGroupId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_commodities_CommodidyId",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_commodities",
                table: "commodities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_commodidyGroups",
                table: "commodidyGroups");

            migrationBuilder.RenameTable(
                name: "commodities",
                newName: "Commodities");

            migrationBuilder.RenameTable(
                name: "commodidyGroups",
                newName: "CommodidyGroups");

            migrationBuilder.RenameIndex(
                name: "IX_commodidyGroups_ParentCommodityGroupId",
                table: "CommodidyGroups",
                newName: "IX_CommodidyGroups_ParentCommodityGroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Commodities",
                table: "Commodities",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommodidyGroups",
                table: "CommodidyGroups",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CommodidyGroups_CommodidyGroups_ParentCommodityGroupId",
                table: "CommodidyGroups",
                column: "ParentCommodityGroupId",
                principalTable: "CommodidyGroups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_CommodidyGroups_CommodityGroupId",
                table: "Products",
                column: "CommodityGroupId",
                principalTable: "CommodidyGroups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Commodities_CommodidyId",
                table: "Products",
                column: "CommodidyId",
                principalTable: "Commodities",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommodidyGroups_CommodidyGroups_ParentCommodityGroupId",
                table: "CommodidyGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_CommodidyGroups_CommodityGroupId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Commodities_CommodidyId",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Commodities",
                table: "Commodities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CommodidyGroups",
                table: "CommodidyGroups");

            migrationBuilder.RenameTable(
                name: "Commodities",
                newName: "commodities");

            migrationBuilder.RenameTable(
                name: "CommodidyGroups",
                newName: "commodidyGroups");

            migrationBuilder.RenameIndex(
                name: "IX_CommodidyGroups_ParentCommodityGroupId",
                table: "commodidyGroups",
                newName: "IX_commodidyGroups_ParentCommodityGroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_commodities",
                table: "commodities",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_commodidyGroups",
                table: "commodidyGroups",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_commodidyGroups_commodidyGroups_ParentCommodityGroupId",
                table: "commodidyGroups",
                column: "ParentCommodityGroupId",
                principalTable: "commodidyGroups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_commodidyGroups_CommodityGroupId",
                table: "Products",
                column: "CommodityGroupId",
                principalTable: "commodidyGroups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_commodities_CommodidyId",
                table: "Products",
                column: "CommodidyId",
                principalTable: "commodities",
                principalColumn: "Id");
        }
    }
}
