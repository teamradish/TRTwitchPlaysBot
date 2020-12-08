using Microsoft.EntityFrameworkCore.Migrations;

namespace TRBot.Data.Migrations
{
    public partial class RemoveConsoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Inputs",
                schema: "inputs");

            migrationBuilder.DropTable(
                name: "Consoles",
                schema: "consoles");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "consoles");

            migrationBuilder.EnsureSchema(
                name: "inputs");

            migrationBuilder.AlterColumn<int>(
                name: "AutoGrantOnLevel",
                schema: "permissionabilities",
                table: "PermissionAbilities",
                type: "INTEGER",
                nullable: false,
                defaultValue: -1,
                oldClrType: typeof(int),
                oldDefaultValue: -1)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.CreateTable(
                name: "Consoles",
                schema: "consoles",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true, defaultValue: "GameConsole")
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
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AxisValue = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0)
                        .Annotation("Sqlite:Autoincrement", true),
                    ButtonValue = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0)
                        .Annotation("Sqlite:Autoincrement", true),
                    InputType = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0)
                        .Annotation("Sqlite:Autoincrement", true),
                    MaxAxisPercent = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 100)
                        .Annotation("Sqlite:Autoincrement", true),
                    MaxAxisVal = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1)
                        .Annotation("Sqlite:Autoincrement", true),
                    MinAxisVal = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true, defaultValue: ""),
                    console_id = table.Column<int>(type: "INTEGER", nullable: false)
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
    }
}
