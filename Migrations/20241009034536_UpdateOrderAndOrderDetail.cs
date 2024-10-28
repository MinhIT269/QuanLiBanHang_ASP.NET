using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quan_ly_ban_hang.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrderAndOrderDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrderCode",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrderCode",
                table: "OrderDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "OrderDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderCode",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OrderCode",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "OrderDetails");
        }
    }
}
