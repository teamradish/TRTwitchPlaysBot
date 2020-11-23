using Microsoft.EntityFrameworkCore.Migrations;

namespace TRBot.Data.Migrations
{
    public partial class ReaddUserIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAbility_PermissionAbilities_PermAbilityid",
                table: "UserAbility");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAbility_Users_user_id",
                table: "UserAbility");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAbility",
                table: "UserAbility");

            migrationBuilder.DropIndex(
                name: "IX_UserAbility_PermAbilityid",
                table: "UserAbility");

            migrationBuilder.DropIndex(
                name: "IX_UserAbility_user_id",
                table: "UserAbility");

            migrationBuilder.DropColumn(
                name: "PermAbilityid",
                table: "UserAbility");

            migrationBuilder.EnsureSchema(
                name: "userabilities");

            migrationBuilder.RenameTable(
                name: "UserAbility",
                newName: "UserAbilities",
                newSchema: "userabilities");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAbilities",
                schema: "userabilities",
                table: "UserAbilities",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Name",
                schema: "users",
                table: "Users",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionAbilities_Name",
                schema: "permissionabilities",
                table: "PermissionAbilities",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAbilities_PermissionAbilities_permability_id",
                schema: "userabilities",
                table: "UserAbilities",
                column: "permability_id",
                principalSchema: "permissionabilities",
                principalTable: "PermissionAbilities",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAbilities_Users_user_id",
                schema: "userabilities",
                table: "UserAbilities",
                column: "user_id",
                principalSchema: "users",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAbilities_PermissionAbilities_permability_id",
                schema: "userabilities",
                table: "UserAbilities");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAbilities_Users_user_id",
                schema: "userabilities",
                table: "UserAbilities");

            migrationBuilder.DropIndex(
                name: "IX_Users_Name",
                schema: "users",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_PermissionAbilities_Name",
                schema: "permissionabilities",
                table: "PermissionAbilities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAbilities",
                schema: "userabilities",
                table: "UserAbilities");

            migrationBuilder.DropIndex(
                name: "IX_UserAbilities_permability_id",
                schema: "userabilities",
                table: "UserAbilities");

            migrationBuilder.RenameTable(
                name: "UserAbilities",
                schema: "userabilities",
                newName: "UserAbility");

            migrationBuilder.RenameIndex(
                name: "IX_UserAbilities_user_id",
                table: "UserAbility",
                newName: "IX_UserAbility_user_id");

            migrationBuilder.AddColumn<int>(
                name: "PermAbilityid",
                table: "UserAbility",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAbility",
                table: "UserAbility",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_UserAbility_PermAbilityid",
                table: "UserAbility",
                column: "PermAbilityid");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAbility_PermissionAbilities_PermAbilityid",
                table: "UserAbility",
                column: "PermAbilityid",
                principalSchema: "permissionabilities",
                principalTable: "PermissionAbilities",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAbility_Users_user_id",
                table: "UserAbility",
                column: "user_id",
                principalSchema: "users",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
