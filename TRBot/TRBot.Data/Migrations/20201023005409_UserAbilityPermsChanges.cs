using Microsoft.EntityFrameworkCore.Migrations;

namespace TRBot.Data.Migrations
{
    public partial class UserAbilityPermsChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "AutoGrantOnLevel",
                schema: "permissionabilities",
                table: "PermissionAbilities",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldDefaultValue: -1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "AutoGrantOnLevel",
                schema: "permissionabilities",
                table: "PermissionAbilities",
                type: "INTEGER",
                nullable: false,
                defaultValue: -1,
                oldClrType: typeof(int),
                oldType: "INTEGER");
        }
    }
}
