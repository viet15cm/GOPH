using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GOPH.Migrations
{
    /// <inheritdoc />
    public partial class v_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommodidyGroups_CommodidyGroups_ParentCommodityGroupId",
                table: "CommodidyGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_CommodidyGroups_CommodityGroupId",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CommodidyGroups",
                table: "CommodidyGroups");

            migrationBuilder.RenameTable(
                name: "CommodidyGroups",
                newName: "CommodityGroups");

            migrationBuilder.RenameIndex(
                name: "IX_CommodidyGroups_ParentCommodityGroupId",
                table: "CommodityGroups",
                newName: "IX_CommodityGroups_ParentCommodityGroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommodityGroups",
                table: "CommodityGroups",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CommodityGroups_CommodityGroups_ParentCommodityGroupId",
                table: "CommodityGroups",
                column: "ParentCommodityGroupId",
                principalTable: "CommodityGroups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_CommodityGroups_CommodityGroupId",
                table: "Products",
                column: "CommodityGroupId",
                principalTable: "CommodityGroups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommodityGroups_CommodityGroups_ParentCommodityGroupId",
                table: "CommodityGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_CommodityGroups_CommodityGroupId",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CommodityGroups",
                table: "CommodityGroups");

            migrationBuilder.RenameTable(
                name: "CommodityGroups",
                newName: "CommodidyGroups");

            migrationBuilder.RenameIndex(
                name: "IX_CommodityGroups_ParentCommodityGroupId",
                table: "CommodidyGroups",
                newName: "IX_CommodidyGroups_ParentCommodityGroupId");

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
        }
    }
}
