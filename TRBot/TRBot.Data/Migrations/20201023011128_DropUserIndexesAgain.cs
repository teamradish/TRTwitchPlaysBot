using Microsoft.EntityFrameworkCore.Migrations;

namespace TRBot.Data.Migrations
{
    public partial class DropUserIndexesAgain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Name",
                schema: "users",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_PermissionAbilities_Name",
                schema: "permissionabilities",
                table: "PermissionAbilities");
            
            migrationBuilder.DropIndex(
                name: "IX_UserAbilities_user_id",
                schema: "userabilities",
                table: "UserAbilities");

            migrationBuilder.DropIndex(
                name: "IX_UserAbilities_permability_id",
                schema: "userabilities",
                table: "UserAbilities");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
