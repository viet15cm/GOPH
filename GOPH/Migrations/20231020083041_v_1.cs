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
            migrationBuilder.CreateTable(
                name: "commodidyGroups",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentCommodityGroupId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_commodidyGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_commodidyGroups_commodidyGroups_ParentCommodityGroupId",
                        column: x => x.ParentCommodityGroupId,
                        principalTable: "commodidyGroups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "commodities",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_commodities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PriceTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UrlImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommodidyId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CommodityGroupId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_commodidyGroups_CommodityGroupId",
                        column: x => x.CommodityGroupId,
                        principalTable: "commodidyGroups",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Products_commodities_CommodidyId",
                        column: x => x.CommodidyId,
                        principalTable: "commodities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_commodidyGroups_ParentCommodityGroupId",
                table: "commodidyGroups",
                column: "ParentCommodityGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CommodidyId",
                table: "Products",
                column: "CommodidyId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CommodityGroupId",
                table: "Products",
                column: "CommodityGroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "commodidyGroups");

            migrationBuilder.DropTable(
                name: "commodities");
        }
    }
}
