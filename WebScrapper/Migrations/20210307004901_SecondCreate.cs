using Microsoft.EntityFrameworkCore.Migrations;

namespace WebScrapper.Migrations
{
    public partial class SecondCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Hash",
                table: "Product",
                type: "TEXT",
                nullable: true);

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

            migrationBuilder.DropColumn(
                name: "Hash",
                table: "Product");
        }
    }
}
