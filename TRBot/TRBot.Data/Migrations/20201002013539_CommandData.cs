using Microsoft.EntityFrameworkCore.Migrations;

namespace TRBot.Data.Migrations
{
    public partial class CommandData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "commanddata");

            migrationBuilder.CreateTable(
                name: "CommandData",
                schema: "commanddata",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(nullable: true),
                    class_name = table.Column<string>(nullable: true),
                    level = table.Column<int>(nullable: false),
                    value_str = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommandData", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommandData_name",
                schema: "commanddata",
                table: "CommandData",
                column: "name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommandData",
                schema: "commanddata");
        }
    }
}
