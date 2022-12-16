using Microsoft.EntityFrameworkCore.Migrations;

namespace HPages.Migrations
{
    public partial class _0001 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Images_ImageId",
                table: "Images");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Images_ImageId",
                table: "Images",
                column: "ImageId");
        }
    }
}
