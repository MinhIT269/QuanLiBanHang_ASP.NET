using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quan_ly_ban_hang.Migrations
{
    /// <inheritdoc />
    public partial class Update_db1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CardId",
                table: "ShoppingCarts",
                newName: "CartId");

            migrationBuilder.RenameColumn(
                name: "CartitemId",
                table: "CartItems",
                newName: "CartItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CartId",
                table: "ShoppingCarts",
                newName: "CardId");

            migrationBuilder.RenameColumn(
                name: "CartItemId",
                table: "CartItems",
                newName: "CartitemId");
        }
    }
}
