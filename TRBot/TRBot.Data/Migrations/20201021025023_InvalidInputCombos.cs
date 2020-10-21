using Microsoft.EntityFrameworkCore.Migrations;

namespace TRBot.Data.Migrations
{
    public partial class InvalidInputCombos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "invalidinputcombos");

            migrationBuilder.CreateTable(
                name: "InvalidInputCombos",
                schema: "invalidinputcombos",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    input_id = table.Column<int>(type: "INTEGER", nullable: false),
                    GameConsoleid = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvalidInputCombos", x => x.id);
                    table.ForeignKey(
                        name: "FK_InvalidInputCombos_Consoles_GameConsoleid",
                        column: x => x.GameConsoleid,
                        principalSchema: "consoles",
                        principalTable: "Consoles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvalidInputCombos_Inputs_input_id",
                        column: x => x.input_id,
                        principalSchema: "inputs",
                        principalTable: "Inputs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvalidInputCombos_GameConsoleid",
                schema: "invalidinputcombos",
                table: "InvalidInputCombos",
                column: "GameConsoleid");

            migrationBuilder.CreateIndex(
                name: "IX_InvalidInputCombos_input_id",
                schema: "invalidinputcombos",
                table: "InvalidInputCombos",
                column: "input_id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvalidInputCombos",
                schema: "invalidinputcombos");
        }
    }
}
