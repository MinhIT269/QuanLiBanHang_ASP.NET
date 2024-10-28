using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quan_ly_ban_hang.Migrations
{
    /// <inheritdoc />
    public partial class InitialProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_ShoppingCarts_CartId",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Customers_CustomerId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Customers_CustomerId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCarts_Customers_CustomerId",
                table: "ShoppingCarts");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_CartId",
                table: "CartItems");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "ShoppingCarts",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ShoppingCarts_CustomerId",
                table: "ShoppingCarts",
                newName: "IX_ShoppingCarts_UserId");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "Reviews",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_CustomerId",
                table: "Reviews",
                newName: "IX_Reviews_UserId");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "Orders",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                newName: "IX_Orders_UserId");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)");

            migrationBuilder.AlterColumn<string>(
                name: "Image",
                table: "Products",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Products",
                type: "nvarchar(2500)",
                maxLength: 2500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2500)",
                oldMaxLength: 2500);

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "ProductImages",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Payments",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)");

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitPrice",
                table: "OrderDetails",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Categories",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<Guid>(
                name: "Shopping_CartCartId",
                table: "CartItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Brands",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300);

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_Shopping_CartCartId",
                table: "CartItems",
                column: "Shopping_CartCartId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_ShoppingCarts_Shopping_CartCartId",
                table: "CartItems",
                column: "Shopping_CartCartId",
                principalTable: "ShoppingCarts",
                principalColumn: "CartId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Nguoi dung_UserId",
                table: "Orders",
                column: "UserId",
                principalTable: "Nguoi dung",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Nguoi dung_UserId",
                table: "Reviews",
                column: "UserId",
                principalTable: "Nguoi dung",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCarts_Nguoi dung_UserId",
                table: "ShoppingCarts",
                column: "UserId",
                principalTable: "Nguoi dung",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_ShoppingCarts_Shopping_CartCartId",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Nguoi dung_UserId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Nguoi dung_UserId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCarts_Nguoi dung_UserId",
                table: "ShoppingCarts");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_Shopping_CartCartId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "Shopping_CartCartId",
                table: "CartItems");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "ShoppingCarts",
                newName: "CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_ShoppingCarts_UserId",
                table: "ShoppingCarts",
                newName: "IX_ShoppingCarts_CustomerId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Reviews",
                newName: "CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_UserId",
                table: "Reviews",
                newName: "IX_Reviews_CustomerId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Orders",
                newName: "CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                newName: "IX_Orders_CustomerId");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Products",
                type: "decimal(18,3)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Image",
                table: "Products",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Products",
                type: "nvarchar(2500)",
                maxLength: 2500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(2500)",
                oldMaxLength: 2500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "ProductImages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Payments",
                type: "decimal(18,3)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "Orders",
                type: "decimal(18,3)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitPrice",
                table: "OrderDetails",
                type: "decimal(18,3)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Categories",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Brands",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CartId",
                table: "CartItems",
                column: "CartId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_ShoppingCarts_CartId",
                table: "CartItems",
                column: "CartId",
                principalTable: "ShoppingCarts",
                principalColumn: "CartId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Customers_CustomerId",
                table: "Orders",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Customers_CustomerId",
                table: "Reviews",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCarts_Customers_CustomerId",
                table: "ShoppingCarts",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
