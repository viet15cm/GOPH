using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GOPH.Migrations
{
    /// <inheritdoc />
    public partial class v_13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrice",
                table: "IssueAnInvoices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "IssueAnInvoices");
        }
    }
}
