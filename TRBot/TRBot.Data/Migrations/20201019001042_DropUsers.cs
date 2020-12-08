using Microsoft.EntityFrameworkCore.Migrations;

namespace TRBot.Data.Migrations
{
    public partial class DropUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserAbilities",
                schema: "userabilities");

            migrationBuilder.DropTable(
                name: "UserStats",
                schema: "userstats");

            migrationBuilder.DropTable(
                name: "PermissionAbilities",
                schema: "permissionabilities");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "users");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "users");

            migrationBuilder.EnsureSchema(
                name: "userstats");

            migrationBuilder.EnsureSchema(
                name: "permissionabilities");

            migrationBuilder.EnsureSchema(
                name: "userabilities");

            migrationBuilder.AlterColumn<int>(
                name: "InputType",
                schema: "inputs",
                table: "Inputs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldDefaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.CreateTable(
                name: "PermissionAbilities",
                schema: "permissionabilities",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AutoGrantOnLevel = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: -1)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
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
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ControllerPort = table.Column<long>(type: "INTEGER", nullable: false),
                    Level = table.Column<long>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "UserAbilities",
                schema: "userabilities",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    permability_id = table.Column<int>(type: "INTEGER", nullable: false),
                    user_id = table.Column<int>(type: "INTEGER", nullable: false)
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
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AutoPromoted = table.Column<long>(type: "INTEGER", nullable: false),
                    BetCounter = table.Column<long>(type: "INTEGER", nullable: false),
                    Credits = table.Column<long>(type: "INTEGER", nullable: false),
                    IsSubscriber = table.Column<long>(type: "INTEGER", nullable: false),
                    OptedOut = table.Column<long>(type: "INTEGER", nullable: false),
                    TotalMessageCount = table.Column<long>(type: "INTEGER", nullable: false),
                    ValidInputCount = table.Column<long>(type: "INTEGER", nullable: false),
                    user_id = table.Column<int>(type: "INTEGER", nullable: false)
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
    }
}
