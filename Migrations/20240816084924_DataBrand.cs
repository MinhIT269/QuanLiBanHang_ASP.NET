using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quan_ly_ban_hang.Migrations
{
	public partial class DataBrand : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			// Thêm bảng Brands trước tiên
			migrationBuilder.CreateTable(
				name: "Brands",
				columns: table => new
				{
					BrandId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
					Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Brands", x => x.BrandId);
				});

			// Thêm cột BrandId vào bảng Products
			migrationBuilder.AddColumn<Guid>(
				name: "BrandId",
				table: "Products",
				type: "uniqueidentifier",
				nullable: true);

			// Cập nhật giá trị BrandId trong Products để không vi phạm khóa ngoại
			migrationBuilder.Sql("UPDATE Products SET BrandId = (SELECT TOP 1 BrandId FROM Brands)");

			// Tạo chỉ mục cho cột BrandId
			migrationBuilder.CreateIndex(
				name: "IX_Products_BrandId",
				table: "Products",
				column: "BrandId");

			// Thêm khóa ngoại
			migrationBuilder.AddForeignKey(
				name: "FK_Products_Brands_BrandId",
				table: "Products",
				column: "BrandId",
				principalTable: "Brands",
				principalColumn: "BrandId",
				onDelete: ReferentialAction.Cascade);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_Products_Brands_BrandId",
				table: "Products");

			migrationBuilder.DropIndex(
				name: "IX_Products_BrandId",
				table: "Products");

			migrationBuilder.DropColumn(
				name: "BrandId",
				table: "Products");

			migrationBuilder.DropTable(
				name: "Brands");
		}
	}
}
