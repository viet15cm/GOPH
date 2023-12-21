using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GOPH.Migrations
{
    /// <inheritdoc />
    public partial class v_10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isUnworn",
                table: "Vouchers");

            migrationBuilder.AddColumn<string>(
                name: "CodeVoucher",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CodeVoucher",
                table: "Orders",
                column: "CodeVoucher",
                unique: true,
                filter: "[CodeVoucher] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Vouchers_CodeVoucher",
                table: "Orders",
                column: "CodeVoucher",
                principalTable: "Vouchers",
                principalColumn: "Code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Vouchers_CodeVoucher",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CodeVoucher",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CodeVoucher",
                table: "Orders");

            migrationBuilder.AddColumn<bool>(
                name: "isUnworn",
                table: "Vouchers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
