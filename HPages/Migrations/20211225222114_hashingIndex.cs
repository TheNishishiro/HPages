using Microsoft.EntityFrameworkCore.Migrations;

namespace HPages.Migrations
{
    public partial class hashingIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Images_Hash",
                table: "Images",
                column: "Hash");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Images_Hash",
                table: "Images");
        }
    }
}
