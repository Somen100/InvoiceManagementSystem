﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvoiceMgmt.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddProductNavigationToInvoiceItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItems_ProductId",
                table: "InvoiceItems",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceItems_Products_ProductId",
                table: "InvoiceItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceItems_Products_ProductId",
                table: "InvoiceItems");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceItems_ProductId",
                table: "InvoiceItems");
        }
    }
}
