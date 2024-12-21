using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvoiceMgmt.DAL.Migrations
{
    /// <inheritdoc />
    public partial class updated_GST_Disc_columns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GSTPercentage",
                table: "Invoices");

            migrationBuilder.AddColumn<decimal>(
                name: "GSTPercentage",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GSTPercentage",
                table: "Products");

            migrationBuilder.AddColumn<decimal>(
                name: "GSTPercentage",
                table: "Invoices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
