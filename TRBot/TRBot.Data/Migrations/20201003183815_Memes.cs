using Microsoft.EntityFrameworkCore.Migrations;

namespace TRBot.Data.Migrations
{
    public partial class Memes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "memes");

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

            migrationBuilder.CreateIndex(
                name: "IX_Memes_MemeName",
                schema: "memes",
                table: "Memes",
                column: "MemeName",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Memes",
                schema: "memes");
        }
    }
}
