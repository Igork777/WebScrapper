using Microsoft.EntityFrameworkCore.Migrations;

namespace WebScrapper.Migrations
{
    public partial class NextCreate22 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_ProductType_ProductTypeId",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_Website_WebsiteId",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_Hash",
                table: "Product");

            migrationBuilder.AlterColumn<int>(
                name: "WebsiteId",
                table: "Product",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "ProductTypeId",
                table: "Product",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_ProductType_ProductTypeId",
                table: "Product",
                column: "ProductTypeId",
                principalTable: "ProductType",
                principalColumn: "ProductTypeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Website_WebsiteId",
                table: "Product",
                column: "WebsiteId",
                principalTable: "Website",
                principalColumn: "WebsiteId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_ProductType_ProductTypeId",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_Website_WebsiteId",
                table: "Product");

            migrationBuilder.AlterColumn<int>(
                name: "WebsiteId",
                table: "Product",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProductTypeId",
                table: "Product",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Product_Hash",
                table: "Product",
                column: "Hash",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Product_ProductType_ProductTypeId",
                table: "Product",
                column: "ProductTypeId",
                principalTable: "ProductType",
                principalColumn: "ProductTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Website_WebsiteId",
                table: "Product",
                column: "WebsiteId",
                principalTable: "Website",
                principalColumn: "WebsiteId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
