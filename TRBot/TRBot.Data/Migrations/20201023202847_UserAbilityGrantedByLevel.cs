using Microsoft.EntityFrameworkCore.Migrations;

namespace TRBot.Data.Migrations
{
    public partial class UserAbilityGrantedByLevel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GivenByLevel",
                schema: "userabilities",
                table: "UserAbilities",
                newName: "GrantedByLevel");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GrantedByLevel",
                schema: "userabilities",
                table: "UserAbilities",
                newName: "GivenByLevel");
        }
    }
}
