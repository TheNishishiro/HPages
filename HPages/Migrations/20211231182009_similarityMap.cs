using Microsoft.EntityFrameworkCore.Migrations;

namespace HPages.Migrations
{
    public partial class similarityMap : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Images_Hash",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_ImageId_UploadDate",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "HasHash",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "Hash",
                table: "Images");

            migrationBuilder.CreateTable(
                name: "SimilarityScores",
                columns: table => new
                {
                    SimilarityId = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChildImageId = table.Column<long>(nullable: false),
                    SimilarityScore = table.Column<float>(nullable: false),
                    ImageId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimilarityScores", x => x.SimilarityId);
                    table.ForeignKey(
                        name: "FK_SimilarityScores_Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Images",
                        principalColumn: "ImageId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Images_ImageId",
                table: "Images",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_UploadDate",
                table: "Images",
                column: "UploadDate");

            migrationBuilder.CreateIndex(
                name: "IX_SimilarityScores_ChildImageId",
                table: "SimilarityScores",
                column: "ChildImageId");

            migrationBuilder.CreateIndex(
                name: "IX_SimilarityScores_ImageId",
                table: "SimilarityScores",
                column: "ImageId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SimilarityScores");

            migrationBuilder.DropIndex(
                name: "IX_Images_ImageId",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_UploadDate",
                table: "Images");

            migrationBuilder.AddColumn<bool>(
                name: "HasHash",
                table: "Images",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Hash",
                table: "Images",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Images_Hash",
                table: "Images",
                column: "Hash");

            migrationBuilder.CreateIndex(
                name: "IX_Images_ImageId_UploadDate",
                table: "Images",
                columns: new[] { "ImageId", "UploadDate" });
        }
    }
}
