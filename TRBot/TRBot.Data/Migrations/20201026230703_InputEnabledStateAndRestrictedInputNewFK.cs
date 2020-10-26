using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TRBot.Data.Migrations
{
    public partial class InputEnabledStateAndRestrictedInputNewFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RestrictedInput");

            migrationBuilder.EnsureSchema(
                name: "restrictedinputs");

            migrationBuilder.AddColumn<long>(
                name: "enabled",
                schema: "inputs",
                table: "Inputs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "RestrictedInputs",
                schema: "restrictedinputs",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    user_id = table.Column<int>(type: "INTEGER", nullable: false),
                    input_id = table.Column<int>(type: "INTEGER", nullable: false),
                    expiration = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestrictedInputs", x => x.id);
                    table.ForeignKey(
                        name: "FK_RestrictedInputs_Inputs_input_id",
                        column: x => x.input_id,
                        principalSchema: "inputs",
                        principalTable: "Inputs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RestrictedInputs_Users_user_id",
                        column: x => x.user_id,
                        principalSchema: "users",
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RestrictedInputs_user_id_input_id",
                schema: "restrictedinputs",
                table: "RestrictedInputs",
                columns: new[] { "user_id", "input_id" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RestrictedInputs",
                schema: "restrictedinputs");

            migrationBuilder.DropColumn(
                name: "enabled",
                schema: "inputs",
                table: "Inputs");

            migrationBuilder.CreateTable(
                name: "RestrictedInput",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    expiration = table.Column<DateTime>(type: "TEXT", nullable: true),
                    input_name = table.Column<string>(type: "TEXT", nullable: true),
                    user_id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestrictedInput", x => x.id);
                    table.ForeignKey(
                        name: "FK_RestrictedInput_Users_user_id",
                        column: x => x.user_id,
                        principalSchema: "users",
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RestrictedInput_user_id",
                table: "RestrictedInput",
                column: "user_id");
        }
    }
}
