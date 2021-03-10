using Microsoft.EntityFrameworkCore.Migrations;

namespace WebScrapper.Migrations
{
    public partial class NextCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Product_WebsiteId_ProductTypeId_Name_Size",
                table: "Product");

            migrationBuilder.CreateIndex(
                name: "IX_Product_WebsiteId",
                table: "Product",
                column: "WebsiteId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Product_WebsiteId",
                table: "Product");

            migrationBuilder.CreateIndex(
                name: "IX_Product_WebsiteId_ProductTypeId_Name_Size",
                table: "Product",
                columns: new[] { "WebsiteId", "ProductTypeId", "Name", "Size" },
                unique: true);
        }
    }
}
