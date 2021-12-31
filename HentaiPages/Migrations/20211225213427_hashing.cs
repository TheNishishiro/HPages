using Microsoft.EntityFrameworkCore.Migrations;

namespace HentaiPages.Migrations
{
    public partial class hashing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasHash",
                table: "Images",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Hash",
                table: "Images",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasHash",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "Hash",
                table: "Images");
        }
    }
}
