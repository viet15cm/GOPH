using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GOPH.Migrations
{
    /// <inheritdoc />
    public partial class v_11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_IssueAnInvoices_IssueAnInvoiceId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Receivers_ReceiverId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Vouchers_CodeVoucher",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CodeVoucher",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_IssueAnInvoiceId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ReceiverId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CodeVoucher",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "IssueAnInvoiceId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ReceiverId",
                table: "Orders");

            migrationBuilder.AddColumn<string>(
                name: "OrderId",
                table: "Vouchers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrderId",
                table: "Receivers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrderId",
                table: "IssueAnInvoices",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrderId",
                table: "Customers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_OrderId",
                table: "Vouchers",
                column: "OrderId",
                unique: true,
                filter: "[OrderId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Receivers_OrderId",
                table: "Receivers",
                column: "OrderId",
                unique: true,
                filter: "[OrderId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueAnInvoices_OrderId",
                table: "IssueAnInvoices",
                column: "OrderId",
                unique: true,
                filter: "[OrderId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_OrderId",
                table: "Customers",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Orders_OrderId",
                table: "Customers",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IssueAnInvoices_Orders_OrderId",
                table: "IssueAnInvoices",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Receivers_Orders_OrderId",
                table: "Receivers",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vouchers_Orders_OrderId",
                table: "Vouchers",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Orders_OrderId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_IssueAnInvoices_Orders_OrderId",
                table: "IssueAnInvoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Receivers_Orders_OrderId",
                table: "Receivers");

            migrationBuilder.DropForeignKey(
                name: "FK_Vouchers_Orders_OrderId",
                table: "Vouchers");

            migrationBuilder.DropIndex(
                name: "IX_Vouchers_OrderId",
                table: "Vouchers");

            migrationBuilder.DropIndex(
                name: "IX_Receivers_OrderId",
                table: "Receivers");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_IssueAnInvoices_OrderId",
                table: "IssueAnInvoices");

            migrationBuilder.DropIndex(
                name: "IX_Customers_OrderId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Receivers");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "IssueAnInvoices");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Customers");

            migrationBuilder.AddColumn<string>(
                name: "CodeVoucher",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IssueAnInvoiceId",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceiverId",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CodeVoucher",
                table: "Orders",
                column: "CodeVoucher",
                unique: true,
                filter: "[CodeVoucher] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId",
                unique: true,
                filter: "[CustomerId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_IssueAnInvoiceId",
                table: "Orders",
                column: "IssueAnInvoiceId",
                unique: true,
                filter: "[IssueAnInvoiceId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ReceiverId",
                table: "Orders",
                column: "ReceiverId",
                unique: true,
                filter: "[ReceiverId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_IssueAnInvoices_IssueAnInvoiceId",
                table: "Orders",
                column: "IssueAnInvoiceId",
                principalTable: "IssueAnInvoices",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Receivers_ReceiverId",
                table: "Orders",
                column: "ReceiverId",
                principalTable: "Receivers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Vouchers_CodeVoucher",
                table: "Orders",
                column: "CodeVoucher",
                principalTable: "Vouchers",
                principalColumn: "Code");
        }
    }
}
