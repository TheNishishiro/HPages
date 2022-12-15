using Microsoft.EntityFrameworkCore.Migrations;

namespace HentaiPages.Migrations
{
    public partial class hashv2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PixelData",
                table: "Images",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Images_PixelData",
                table: "Images",
                column: "PixelData");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Images_PixelData",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "PixelData",
                table: "Images");
        }
    }
}
