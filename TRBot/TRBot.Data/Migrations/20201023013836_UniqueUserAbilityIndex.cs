using Microsoft.EntityFrameworkCore.Migrations;

namespace TRBot.Data.Migrations
{
    public partial class UniqueUserAbilityIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserAbilities_user_id_permability_id",
                schema: "userabilities",
                table: "UserAbilities",
                columns: new[] { "user_id", "permability_id" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserAbilities_user_id_permability_id",
                schema: "userabilities",
                table: "UserAbilities");
        }
    }
}
