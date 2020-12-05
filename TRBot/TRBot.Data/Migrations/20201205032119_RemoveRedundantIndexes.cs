using Microsoft.EntityFrameworkCore.Migrations;

namespace TRBot.Data.Migrations
{
    public partial class RemoveRedundantIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserAbilities_PermAbilityID",
                table: "UserAbilities");

            migrationBuilder.DropIndex(
                name: "IX_RestrictedInputs_InputID",
                table: "RestrictedInputs");
            
            migrationBuilder.DropIndex(
                name: "IX_Inputs_ConsoleID",
                table: "Inputs");

            migrationBuilder.DropIndex(
                name: "IX_InvalidInputCombos_GameConsoleID",
                table: "InvalidInputCombos");    
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserAbilities_PermAbilityID",
                schema: "userabilities",
                table: "UserAbilities",
                column: "PermabilityID");

            migrationBuilder.CreateIndex(
                name: "IX_RestrictedInputs_InputID",
                schema: "restrictedinputs",
                table: "RestrictedInputs",
                column: "InputID");
            
            migrationBuilder.CreateIndex(
                name: "IX_Inputs_ConsoleID",
                schema: "inputs",
                table: "Inputs",
                column: "ConsoleID");

            migrationBuilder.CreateIndex(
                name: "IX_InvalidInputCombos_GameConsoleID",
                schema: "invalidinputcombos",
                table: "InvalidInputCombos",
                column: "GameConsoleID");
        }
    }
}
