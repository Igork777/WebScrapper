using Microsoft.EntityFrameworkCore.Migrations;

namespace WebScrapper.Migrations
{
    public partial class renewedDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Product_Hash",
                table: "Product",
                column: "Hash",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Product_Hash",
                table: "Product");
        }
    }
}
