using Microsoft.EntityFrameworkCore.Migrations;

namespace TRBot.Data.Migrations
{
    public partial class UserSimulateData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "OptedInSimulate",
                schema: "userstats",
                table: "UserStats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "SimulateHistory",
                schema: "userstats",
                table: "UserStats",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OptedInSimulate",
                schema: "userstats",
                table: "UserStats");

            migrationBuilder.DropColumn(
                name: "SimulateHistory",
                schema: "userstats",
                table: "UserStats");
        }
    }
}
