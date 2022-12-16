using Microsoft.EntityFrameworkCore.Migrations;

namespace HPages.Migrations
{
    public partial class similarityParent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentImageId",
                table: "SimilarityScores",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentImageId",
                table: "SimilarityScores");
        }
    }
}
