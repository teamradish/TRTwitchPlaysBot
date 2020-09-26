using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TRBot.Data.Migrations
{
    public partial class SavestateLogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "savestatelogs");

            migrationBuilder.CreateTable(
                name: "SavestateLogs",
                schema: "savestatelogs",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LogDateTime = table.Column<DateTime>(nullable: false),
                    User = table.Column<string>(nullable: true),
                    LogMessage = table.Column<string>(nullable: true),
                    SavestateNum = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavestateLogs", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SavestateLogs",
                schema: "savestatelogs");
        }
    }
}
