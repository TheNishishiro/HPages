using Microsoft.EntityFrameworkCore.Migrations;

namespace HentaiPages.Migrations
{
    public partial class similarityRelationsCascade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SimilarityScores_Images_ImageId",
                table: "SimilarityScores");

            migrationBuilder.AddForeignKey(
                name: "FK_SimilarityScores_Images_ImageId",
                table: "SimilarityScores",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "ImageId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SimilarityScores_Images_ImageId",
                table: "SimilarityScores");

            migrationBuilder.AddForeignKey(
                name: "FK_SimilarityScores_Images_ImageId",
                table: "SimilarityScores",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "ImageId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
