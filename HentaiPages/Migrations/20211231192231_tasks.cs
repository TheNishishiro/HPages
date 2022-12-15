using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HentaiPages.Migrations
{
    public partial class tasks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    WorkerTaskId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ObjectId = table.Column<int>(type: "INTEGER", nullable: true),
                    ObjectId2 = table.Column<int>(type: "INTEGER", nullable: true),
                    ResultId = table.Column<int>(type: "INTEGER", nullable: true),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    ErrorMessage = table.Column<string>(type: "TEXT", nullable: true),
                    ResultObjectId = table.Column<int>(type: "INTEGER", nullable: true),
                    ResultMessage = table.Column<string>(type: "TEXT", nullable: true),
                    PostDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FinishDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RestartOnFailure = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.WorkerTaskId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tasks");
        }
    }
}
