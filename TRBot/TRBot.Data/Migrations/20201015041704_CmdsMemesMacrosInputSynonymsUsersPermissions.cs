using Microsoft.EntityFrameworkCore.Migrations;

namespace TRBot.Data.Migrations
{
    public partial class CmdsMemesMacrosInputSynonymsUsersPermissions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "consoles");

            migrationBuilder.EnsureSchema(
                name: "inputs");

            migrationBuilder.EnsureSchema(
                name: "commanddata");

            migrationBuilder.EnsureSchema(
                name: "memes");

            migrationBuilder.EnsureSchema(
                name: "users");

            migrationBuilder.EnsureSchema(
                name: "userstats");

            migrationBuilder.EnsureSchema(
                name: "macros");

            migrationBuilder.EnsureSchema(
                name: "inputsynonyms");

            migrationBuilder.EnsureSchema(
                name: "permissionabilities");

            migrationBuilder.EnsureSchema(
                name: "userabilities");

            migrationBuilder.CreateTable(
                name: "CommandData",
                schema: "commanddata",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(nullable: true),
                    class_name = table.Column<string>(nullable: true),
                    level = table.Column<long>(nullable: false, defaultValue: 0L)
                        .Annotation("Sqlite:Autoincrement", true),
                    enabled = table.Column<long>(nullable: false),
                    display_in_list = table.Column<long>(nullable: false),
                    value_str = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommandData", x => x.id);
                });

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
                name: "InputSynonyms",
                schema: "inputsynonyms",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    console_id = table.Column<int>(nullable: false, defaultValue: 1)
                        .Annotation("Sqlite:Autoincrement", true),
                    SynonymName = table.Column<string>(nullable: true, defaultValue: ""),
                    SynonymValue = table.Column<string>(nullable: true, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InputSynonyms", x => x.id);
                });

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

            migrationBuilder.CreateTable(
                name: "Memes",
                schema: "memes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MemeName = table.Column<string>(nullable: true),
                    MemeValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Memes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PermissionAbilities",
                schema: "permissionabilities",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AutoGrantOnLevel = table.Column<int>(nullable: false, defaultValue: -1),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionAbilities", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "users",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true, defaultValue: ""),
                    Level = table.Column<long>(nullable: false),
                    ControllerPort = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.id);
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

            migrationBuilder.CreateTable(
                name: "UserAbilities",
                schema: "userabilities",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    user_id = table.Column<int>(nullable: false),
                    permability_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAbilities", x => x.id);
                    table.ForeignKey(
                        name: "FK_UserAbilities_PermissionAbilities_permability_id",
                        column: x => x.permability_id,
                        principalSchema: "permissionabilities",
                        principalTable: "PermissionAbilities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAbilities_Users_user_id",
                        column: x => x.user_id,
                        principalSchema: "users",
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserStats",
                schema: "userstats",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    user_id = table.Column<int>(nullable: false),
                    Credits = table.Column<long>(nullable: false),
                    TotalMessageCount = table.Column<long>(nullable: false),
                    ValidInputCount = table.Column<long>(nullable: false),
                    IsSubscriber = table.Column<long>(nullable: false),
                    BetCounter = table.Column<long>(nullable: false),
                    AutoPromoted = table.Column<long>(nullable: false),
                    OptedOut = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserStats", x => x.id);
                    table.ForeignKey(
                        name: "FK_UserStats_Users_user_id",
                        column: x => x.user_id,
                        principalSchema: "users",
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommandData_name",
                schema: "commanddata",
                table: "CommandData",
                column: "name",
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_InputSynonyms_SynonymName_console_id",
                schema: "inputsynonyms",
                table: "InputSynonyms",
                columns: new[] { "SynonymName", "console_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Macros_MacroName",
                schema: "macros",
                table: "Macros",
                column: "MacroName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Memes_MemeName",
                schema: "memes",
                table: "Memes",
                column: "MemeName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PermissionAbilities_Name",
                schema: "permissionabilities",
                table: "PermissionAbilities",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAbilities_permability_id",
                schema: "userabilities",
                table: "UserAbilities",
                column: "permability_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAbilities_user_id",
                schema: "userabilities",
                table: "UserAbilities",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Name",
                schema: "users",
                table: "Users",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_UserStats_user_id",
                schema: "userstats",
                table: "UserStats",
                column: "user_id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommandData",
                schema: "commanddata");

            migrationBuilder.DropTable(
                name: "Inputs",
                schema: "inputs");

            migrationBuilder.DropTable(
                name: "InputSynonyms",
                schema: "inputsynonyms");

            migrationBuilder.DropTable(
                name: "Macros",
                schema: "macros");

            migrationBuilder.DropTable(
                name: "Memes",
                schema: "memes");

            migrationBuilder.DropTable(
                name: "UserAbilities",
                schema: "userabilities");

            migrationBuilder.DropTable(
                name: "UserStats",
                schema: "userstats");

            migrationBuilder.DropTable(
                name: "Consoles",
                schema: "consoles");

            migrationBuilder.DropTable(
                name: "PermissionAbilities",
                schema: "permissionabilities");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "users");
        }
    }
}
