using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvoiceMgmt.DAL.Migrations
{
    /// <inheritdoc />
    public partial class roll_nullable_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Make RoleId nullable in Users table
            migrationBuilder.AlterColumn<int>(
                name: "RoleId",
                table: "Users",
                nullable: true,  // Nullable RoleId for users without roles
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "RoleId",
                onDelete: ReferentialAction.SetNull);  // Ensure RoleId can be null in Users table
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert RoleId to non-nullable in Users table
            migrationBuilder.AlterColumn<int>(
                name: "RoleId",
                table: "Users",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "RoleId",
                onDelete: ReferentialAction.Cascade);  // Set the behavior when RoleId is deleted

        }
    }
}
