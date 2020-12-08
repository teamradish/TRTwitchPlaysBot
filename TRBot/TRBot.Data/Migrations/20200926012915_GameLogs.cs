using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TRBot.Data.Migrations
{
    public partial class GameLogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "gamelogs");

            migrationBuilder.CreateTable(
                name: "GameLogs",
                schema: "gamelogs",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LogDateTime = table.Column<DateTime>(nullable: false),
                    User = table.Column<string>(nullable: true),
                    LogMessage = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameLogs", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameLogs",
                schema: "gamelogs");
        }
    }
}
