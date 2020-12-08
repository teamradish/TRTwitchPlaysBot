using Microsoft.EntityFrameworkCore.Migrations;

namespace TRBot.Data.Migrations
{
    public partial class ConsistentNaming : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inputs_Consoles_console_id",
                schema: "inputs",
                table: "Inputs");

            migrationBuilder.DropForeignKey(
                name: "FK_InvalidInputCombos_Consoles_GameConsoleid",
                schema: "invalidinputcombos",
                table: "InvalidInputCombos");

            migrationBuilder.DropForeignKey(
                name: "FK_InvalidInputCombos_Inputs_input_id",
                schema: "invalidinputcombos",
                table: "InvalidInputCombos");

            migrationBuilder.DropForeignKey(
                name: "FK_RestrictedInputs_Inputs_input_id",
                schema: "restrictedinputs",
                table: "RestrictedInputs");

            migrationBuilder.DropForeignKey(
                name: "FK_RestrictedInputs_Users_user_id",
                schema: "restrictedinputs",
                table: "RestrictedInputs");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAbilities_PermissionAbilities_permability_id",
                schema: "userabilities",
                table: "UserAbilities");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAbilities_Users_user_id",
                schema: "userabilities",
                table: "UserAbilities");

            migrationBuilder.DropForeignKey(
                name: "FK_UserStats_Users_user_id",
                schema: "userstats",
                table: "UserStats");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "userstats",
                table: "UserStats",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "user_id",
                schema: "userstats",
                table: "UserStats",
                newName: "UserID");

            migrationBuilder.RenameIndex(
                name: "IX_UserStats_user_id",
                schema: "userstats",
                table: "UserStats",
                newName: "IX_UserStats_UserID");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "users",
                table: "Users",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "expiration",
                schema: "userabilities",
                table: "UserAbilities",
                newName: "Expiration");

            migrationBuilder.RenameColumn(
                name: "enabled",
                schema: "userabilities",
                table: "UserAbilities",
                newName: "Enabled");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "userabilities",
                table: "UserAbilities",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "value_str",
                schema: "userabilities",
                table: "UserAbilities",
                newName: "ValueStr");

            migrationBuilder.RenameColumn(
                name: "value_int",
                schema: "userabilities",
                table: "UserAbilities",
                newName: "ValueInt");

            migrationBuilder.RenameColumn(
                name: "user_id",
                schema: "userabilities",
                table: "UserAbilities",
                newName: "UserID");

            migrationBuilder.RenameColumn(
                name: "permability_id",
                schema: "userabilities",
                table: "UserAbilities",
                newName: "PermabilityID");

            migrationBuilder.RenameIndex(
                name: "IX_UserAbilities_user_id_permability_id",
                schema: "userabilities",
                table: "UserAbilities",
                newName: "IX_UserAbilities_UserID_PermabilityID");

            migrationBuilder.RenameColumn(
                name: "key",
                schema: "settings",
                table: "Settings",
                newName: "Key");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "settings",
                table: "Settings",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "value_str",
                schema: "settings",
                table: "Settings",
                newName: "ValueStr");

            migrationBuilder.RenameColumn(
                name: "value_int",
                schema: "settings",
                table: "Settings",
                newName: "ValueInt");

            migrationBuilder.RenameIndex(
                name: "IX_Settings_key",
                schema: "settings",
                table: "Settings",
                newName: "IX_Settings_Key");

            migrationBuilder.RenameColumn(
                name: "expiration",
                schema: "restrictedinputs",
                table: "RestrictedInputs",
                newName: "Expiration");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "restrictedinputs",
                table: "RestrictedInputs",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "user_id",
                schema: "restrictedinputs",
                table: "RestrictedInputs",
                newName: "UserID");

            migrationBuilder.RenameColumn(
                name: "input_id",
                schema: "restrictedinputs",
                table: "RestrictedInputs",
                newName: "InputID");

            migrationBuilder.RenameIndex(
                name: "IX_RestrictedInputs_user_id_input_id",
                schema: "restrictedinputs",
                table: "RestrictedInputs",
                newName: "IX_RestrictedInputs_UserID_InputID");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "permissionabilities",
                table: "PermissionAbilities",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "value_str",
                schema: "permissionabilities",
                table: "PermissionAbilities",
                newName: "ValueStr");

            migrationBuilder.RenameColumn(
                name: "value_int",
                schema: "permissionabilities",
                table: "PermissionAbilities",
                newName: "ValueInt");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "macros",
                table: "Macros",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "GameConsoleid",
                schema: "invalidinputcombos",
                table: "InvalidInputCombos",
                newName: "GameConsoleID");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "invalidinputcombos",
                table: "InvalidInputCombos",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "input_id",
                schema: "invalidinputcombos",
                table: "InvalidInputCombos",
                newName: "InputID");

            migrationBuilder.RenameIndex(
                name: "IX_InvalidInputCombos_GameConsoleid",
                schema: "invalidinputcombos",
                table: "InvalidInputCombos",
                newName: "IX_InvalidInputCombos_GameConsoleID");

            migrationBuilder.RenameIndex(
                name: "IX_InvalidInputCombos_input_id",
                schema: "invalidinputcombos",
                table: "InvalidInputCombos",
                newName: "IX_InvalidInputCombos_InputID");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "inputsynonyms",
                table: "InputSynonyms",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "console_id",
                schema: "inputsynonyms",
                table: "InputSynonyms",
                newName: "ConsoleID");

            migrationBuilder.RenameIndex(
                name: "IX_InputSynonyms_SynonymName_console_id",
                schema: "inputsynonyms",
                table: "InputSynonyms",
                newName: "IX_InputSynonyms_SynonymName_ConsoleID");

            migrationBuilder.RenameColumn(
                name: "level",
                schema: "inputs",
                table: "Inputs",
                newName: "Level");

            migrationBuilder.RenameColumn(
                name: "enabled",
                schema: "inputs",
                table: "Inputs",
                newName: "Enabled");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "inputs",
                table: "Inputs",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "console_id",
                schema: "inputs",
                table: "Inputs",
                newName: "ConsoleID");

            migrationBuilder.RenameIndex(
                name: "IX_Inputs_Name_console_id",
                schema: "inputs",
                table: "Inputs",
                newName: "IX_Inputs_Name_ConsoleID");

            migrationBuilder.RenameIndex(
                name: "IX_Inputs_console_id",
                schema: "inputs",
                table: "Inputs",
                newName: "IX_Inputs_ConsoleID");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "gamelogs",
                table: "GameLogs",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "consoles",
                table: "Consoles",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "name",
                schema: "commanddata",
                table: "CommandData",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "level",
                schema: "commanddata",
                table: "CommandData",
                newName: "Level");

            migrationBuilder.RenameColumn(
                name: "enabled",
                schema: "commanddata",
                table: "CommandData",
                newName: "Enabled");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "commanddata",
                table: "CommandData",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "value_str",
                schema: "commanddata",
                table: "CommandData",
                newName: "ValueStr");

            migrationBuilder.RenameColumn(
                name: "display_in_list",
                schema: "commanddata",
                table: "CommandData",
                newName: "DisplayInList");

            migrationBuilder.RenameColumn(
                name: "class_name",
                schema: "commanddata",
                table: "CommandData",
                newName: "ClassName");

            migrationBuilder.RenameIndex(
                name: "IX_CommandData_name",
                schema: "commanddata",
                table: "CommandData",
                newName: "IX_CommandData_Name");

            migrationBuilder.AddForeignKey(
                name: "FK_Inputs_Consoles_ConsoleID",
                schema: "inputs",
                table: "Inputs",
                column: "ConsoleID",
                principalSchema: "consoles",
                principalTable: "Consoles",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvalidInputCombos_Consoles_GameConsoleID",
                schema: "invalidinputcombos",
                table: "InvalidInputCombos",
                column: "GameConsoleID",
                principalSchema: "consoles",
                principalTable: "Consoles",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvalidInputCombos_Inputs_InputID",
                schema: "invalidinputcombos",
                table: "InvalidInputCombos",
                column: "InputID",
                principalSchema: "inputs",
                principalTable: "Inputs",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RestrictedInputs_Inputs_InputID",
                schema: "restrictedinputs",
                table: "RestrictedInputs",
                column: "InputID",
                principalSchema: "inputs",
                principalTable: "Inputs",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RestrictedInputs_Users_UserID",
                schema: "restrictedinputs",
                table: "RestrictedInputs",
                column: "UserID",
                principalSchema: "users",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAbilities_PermissionAbilities_PermabilityID",
                schema: "userabilities",
                table: "UserAbilities",
                column: "PermabilityID",
                principalSchema: "permissionabilities",
                principalTable: "PermissionAbilities",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAbilities_Users_UserID",
                schema: "userabilities",
                table: "UserAbilities",
                column: "UserID",
                principalSchema: "users",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserStats_Users_UserID",
                schema: "userstats",
                table: "UserStats",
                column: "UserID",
                principalSchema: "users",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inputs_Consoles_ConsoleID",
                schema: "inputs",
                table: "Inputs");

            migrationBuilder.DropForeignKey(
                name: "FK_InvalidInputCombos_Consoles_GameConsoleID",
                schema: "invalidinputcombos",
                table: "InvalidInputCombos");

            migrationBuilder.DropForeignKey(
                name: "FK_InvalidInputCombos_Inputs_InputID",
                schema: "invalidinputcombos",
                table: "InvalidInputCombos");

            migrationBuilder.DropForeignKey(
                name: "FK_RestrictedInputs_Inputs_InputID",
                schema: "restrictedinputs",
                table: "RestrictedInputs");

            migrationBuilder.DropForeignKey(
                name: "FK_RestrictedInputs_Users_UserID",
                schema: "restrictedinputs",
                table: "RestrictedInputs");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAbilities_PermissionAbilities_PermabilityID",
                schema: "userabilities",
                table: "UserAbilities");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAbilities_Users_UserID",
                schema: "userabilities",
                table: "UserAbilities");

            migrationBuilder.DropForeignKey(
                name: "FK_UserStats_Users_UserID",
                schema: "userstats",
                table: "UserStats");

            migrationBuilder.RenameColumn(
                name: "ID",
                schema: "userstats",
                table: "UserStats",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserID",
                schema: "userstats",
                table: "UserStats",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "IX_UserStats_UserID",
                schema: "userstats",
                table: "UserStats",
                newName: "IX_UserStats_user_id");

            migrationBuilder.RenameColumn(
                name: "ID",
                schema: "users",
                table: "Users",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Expiration",
                schema: "userabilities",
                table: "UserAbilities",
                newName: "expiration");

            migrationBuilder.RenameColumn(
                name: "Enabled",
                schema: "userabilities",
                table: "UserAbilities",
                newName: "enabled");

            migrationBuilder.RenameColumn(
                name: "ID",
                schema: "userabilities",
                table: "UserAbilities",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ValueStr",
                schema: "userabilities",
                table: "UserAbilities",
                newName: "value_str");

            migrationBuilder.RenameColumn(
                name: "ValueInt",
                schema: "userabilities",
                table: "UserAbilities",
                newName: "value_int");

            migrationBuilder.RenameColumn(
                name: "UserID",
                schema: "userabilities",
                table: "UserAbilities",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "PermabilityID",
                schema: "userabilities",
                table: "UserAbilities",
                newName: "permability_id");

            migrationBuilder.RenameIndex(
                name: "IX_UserAbilities_UserID_PermabilityID",
                schema: "userabilities",
                table: "UserAbilities",
                newName: "IX_UserAbilities_user_id_permability_id");

            migrationBuilder.RenameIndex(
                name: "IX_UserAbilities_PermabilityID",
                schema: "userabilities",
                table: "UserAbilities",
                newName: "IX_UserAbilities_permability_id");

            migrationBuilder.RenameColumn(
                name: "Key",
                schema: "settings",
                table: "Settings",
                newName: "key");

            migrationBuilder.RenameColumn(
                name: "ID",
                schema: "settings",
                table: "Settings",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ValueStr",
                schema: "settings",
                table: "Settings",
                newName: "value_str");

            migrationBuilder.RenameColumn(
                name: "ValueInt",
                schema: "settings",
                table: "Settings",
                newName: "value_int");

            migrationBuilder.RenameIndex(
                name: "IX_Settings_Key",
                schema: "settings",
                table: "Settings",
                newName: "IX_Settings_key");

            migrationBuilder.RenameColumn(
                name: "Expiration",
                schema: "restrictedinputs",
                table: "RestrictedInputs",
                newName: "expiration");

            migrationBuilder.RenameColumn(
                name: "ID",
                schema: "restrictedinputs",
                table: "RestrictedInputs",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserID",
                schema: "restrictedinputs",
                table: "RestrictedInputs",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "InputID",
                schema: "restrictedinputs",
                table: "RestrictedInputs",
                newName: "input_id");

            migrationBuilder.RenameIndex(
                name: "IX_RestrictedInputs_UserID_InputID",
                schema: "restrictedinputs",
                table: "RestrictedInputs",
                newName: "IX_RestrictedInputs_user_id_input_id");

            migrationBuilder.RenameColumn(
                name: "ID",
                schema: "permissionabilities",
                table: "PermissionAbilities",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ValueStr",
                schema: "permissionabilities",
                table: "PermissionAbilities",
                newName: "value_str");

            migrationBuilder.RenameColumn(
                name: "ValueInt",
                schema: "permissionabilities",
                table: "PermissionAbilities",
                newName: "value_int");

            migrationBuilder.RenameColumn(
                name: "ID",
                schema: "macros",
                table: "Macros",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "GameConsoleID",
                schema: "invalidinputcombos",
                table: "InvalidInputCombos",
                newName: "GameConsoleid");

            migrationBuilder.RenameColumn(
                name: "ID",
                schema: "invalidinputcombos",
                table: "InvalidInputCombos",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "InputID",
                schema: "invalidinputcombos",
                table: "InvalidInputCombos",
                newName: "input_id");

            migrationBuilder.RenameIndex(
                name: "IX_InvalidInputCombos_GameConsoleID",
                schema: "invalidinputcombos",
                table: "InvalidInputCombos",
                newName: "IX_InvalidInputCombos_GameConsoleid");

            migrationBuilder.RenameIndex(
                name: "IX_InvalidInputCombos_InputID",
                schema: "invalidinputcombos",
                table: "InvalidInputCombos",
                newName: "IX_InvalidInputCombos_input_id");

            migrationBuilder.RenameColumn(
                name: "ID",
                schema: "inputsynonyms",
                table: "InputSynonyms",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ConsoleID",
                schema: "inputsynonyms",
                table: "InputSynonyms",
                newName: "console_id");

            migrationBuilder.RenameIndex(
                name: "IX_InputSynonyms_SynonymName_ConsoleID",
                schema: "inputsynonyms",
                table: "InputSynonyms",
                newName: "IX_InputSynonyms_SynonymName_console_id");

            migrationBuilder.RenameColumn(
                name: "Level",
                schema: "inputs",
                table: "Inputs",
                newName: "level");

            migrationBuilder.RenameColumn(
                name: "Enabled",
                schema: "inputs",
                table: "Inputs",
                newName: "enabled");

            migrationBuilder.RenameColumn(
                name: "ID",
                schema: "inputs",
                table: "Inputs",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ConsoleID",
                schema: "inputs",
                table: "Inputs",
                newName: "console_id");

            migrationBuilder.RenameIndex(
                name: "IX_Inputs_Name_ConsoleID",
                schema: "inputs",
                table: "Inputs",
                newName: "IX_Inputs_Name_console_id");

            migrationBuilder.RenameIndex(
                name: "IX_Inputs_ConsoleID",
                schema: "inputs",
                table: "Inputs",
                newName: "IX_Inputs_console_id");

            migrationBuilder.RenameColumn(
                name: "ID",
                schema: "gamelogs",
                table: "GameLogs",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ID",
                schema: "consoles",
                table: "Consoles",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "commanddata",
                table: "CommandData",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Level",
                schema: "commanddata",
                table: "CommandData",
                newName: "level");

            migrationBuilder.RenameColumn(
                name: "Enabled",
                schema: "commanddata",
                table: "CommandData",
                newName: "enabled");

            migrationBuilder.RenameColumn(
                name: "ID",
                schema: "commanddata",
                table: "CommandData",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ValueStr",
                schema: "commanddata",
                table: "CommandData",
                newName: "value_str");

            migrationBuilder.RenameColumn(
                name: "DisplayInList",
                schema: "commanddata",
                table: "CommandData",
                newName: "display_in_list");

            migrationBuilder.RenameColumn(
                name: "ClassName",
                schema: "commanddata",
                table: "CommandData",
                newName: "class_name");

            migrationBuilder.RenameIndex(
                name: "IX_CommandData_Name",
                schema: "commanddata",
                table: "CommandData",
                newName: "IX_CommandData_name");

            migrationBuilder.AddForeignKey(
                name: "FK_Inputs_Consoles_console_id",
                schema: "inputs",
                table: "Inputs",
                column: "console_id",
                principalSchema: "consoles",
                principalTable: "Consoles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvalidInputCombos_Consoles_GameConsoleid",
                schema: "invalidinputcombos",
                table: "InvalidInputCombos",
                column: "GameConsoleid",
                principalSchema: "consoles",
                principalTable: "Consoles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvalidInputCombos_Inputs_input_id",
                schema: "invalidinputcombos",
                table: "InvalidInputCombos",
                column: "input_id",
                principalSchema: "inputs",
                principalTable: "Inputs",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RestrictedInputs_Inputs_input_id",
                schema: "restrictedinputs",
                table: "RestrictedInputs",
                column: "input_id",
                principalSchema: "inputs",
                principalTable: "Inputs",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RestrictedInputs_Users_user_id",
                schema: "restrictedinputs",
                table: "RestrictedInputs",
                column: "user_id",
                principalSchema: "users",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

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

            migrationBuilder.AddForeignKey(
                name: "FK_UserStats_Users_user_id",
                schema: "userstats",
                table: "UserStats",
                column: "user_id",
                principalSchema: "users",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
