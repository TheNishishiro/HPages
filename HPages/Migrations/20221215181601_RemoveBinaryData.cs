using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HPages.Migrations
{
    public partial class RemoveBinaryData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Data",
                table: "Images");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Data",
                table: "Images",
                type: "BLOB",
                nullable: true);
        }
    }
}
