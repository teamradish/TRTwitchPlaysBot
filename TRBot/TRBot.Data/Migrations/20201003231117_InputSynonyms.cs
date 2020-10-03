using Microsoft.EntityFrameworkCore.Migrations;

namespace TRBot.Data.Migrations
{
    public partial class InputSynonyms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "inputsynonyms");

            migrationBuilder.CreateTable(
                name: "InputSynonyms",
                schema: "inputsynonyms",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SynonymName = table.Column<string>(nullable: true, defaultValue: ""),
                    SynonymValue = table.Column<string>(nullable: true, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InputSynonyms", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InputSynonyms_SynonymName",
                schema: "inputsynonyms",
                table: "InputSynonyms",
                column: "SynonymName",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InputSynonyms",
                schema: "inputsynonyms");
        }
    }
}
