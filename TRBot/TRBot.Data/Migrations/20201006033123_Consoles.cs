using Microsoft.EntityFrameworkCore.Migrations;

namespace TRBot.Data.Migrations
{
    public partial class Consoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "consoles");

            migrationBuilder.EnsureSchema(
                name: "inputs");

            migrationBuilder.CreateTable(
                name: "Consoles",
                schema: "consoles",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true, defaultValue: "GameConsole")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consoles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Inputs",
                schema: "inputs",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    console_id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true, defaultValue: ""),
                    ButtonValue = table.Column<int>(nullable: false, defaultValue: 0)
                        .Annotation("Sqlite:Autoincrement", true),
                    AxisValue = table.Column<int>(nullable: false, defaultValue: 0)
                        .Annotation("Sqlite:Autoincrement", true),
                    InputType = table.Column<int>(nullable: false, defaultValue: 0),
                    MinAxisVal = table.Column<int>(nullable: false, defaultValue: 0)
                        .Annotation("Sqlite:Autoincrement", true),
                    MaxAxisVal = table.Column<int>(nullable: false, defaultValue: 1)
                        .Annotation("Sqlite:Autoincrement", true),
                    MaxAxisPercent = table.Column<int>(nullable: false, defaultValue: 100)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inputs", x => x.id);
                    table.ForeignKey(
                        name: "FK_Inputs_Consoles_console_id",
                        column: x => x.console_id,
                        principalSchema: "consoles",
                        principalTable: "Consoles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Consoles_Name",
                schema: "consoles",
                table: "Consoles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inputs_console_id",
                schema: "inputs",
                table: "Inputs",
                column: "console_id");

            migrationBuilder.CreateIndex(
                name: "IX_Inputs_Name_console_id",
                schema: "inputs",
                table: "Inputs",
                columns: new[] { "Name", "console_id" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Inputs",
                schema: "inputs");

            migrationBuilder.DropTable(
                name: "Consoles",
                schema: "consoles");
        }
    }
}
