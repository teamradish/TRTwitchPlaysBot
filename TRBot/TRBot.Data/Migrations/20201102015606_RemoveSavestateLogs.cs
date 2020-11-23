using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TRBot.Data.Migrations
{
    public partial class RemoveSavestateLogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SavestateLogs",
                schema: "savestatelogs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "savestatelogs");

            migrationBuilder.CreateTable(
                name: "SavestateLogs",
                schema: "savestatelogs",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LogDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LogMessage = table.Column<string>(type: "TEXT", nullable: true),
                    SavestateNum = table.Column<int>(type: "INTEGER", nullable: false),
                    User = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavestateLogs", x => x.id);
                });
        }
    }
}
