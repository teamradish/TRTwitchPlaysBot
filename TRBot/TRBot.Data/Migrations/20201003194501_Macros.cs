using Microsoft.EntityFrameworkCore.Migrations;

namespace TRBot.Data.Migrations
{
    public partial class Macros : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "macros");

            migrationBuilder.CreateTable(
                name: "Macros",
                schema: "macros",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MacroName = table.Column<string>(nullable: true, defaultValue: ""),
                    MacroValue = table.Column<string>(nullable: true, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Macros", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Macros_MacroName",
                schema: "macros",
                table: "Macros",
                column: "MacroName",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Macros",
                schema: "macros");
        }
    }
}
