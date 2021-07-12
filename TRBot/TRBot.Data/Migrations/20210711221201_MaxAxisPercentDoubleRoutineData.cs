using Microsoft.EntityFrameworkCore.Migrations;

namespace TRBot.Data.Migrations
{
    public partial class MaxAxisPercentDoubleRoutineData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "routinedata");

            migrationBuilder.AlterColumn<double>(
                name: "MaxAxisPercent",
                schema: "inputs",
                table: "Inputs",
                type: "REAL",
                nullable: false,
                defaultValue: 100.0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldDefaultValue: 100);

            migrationBuilder.CreateTable(
                name: "RoutineData",
                schema: "routinedata",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true, defaultValue: ""),
                    ClassName = table.Column<string>(type: "TEXT", nullable: true, defaultValue: ""),
                    Enabled = table.Column<long>(type: "INTEGER", nullable: false, defaultValue: 1L),
                    ResetOnReload = table.Column<long>(type: "INTEGER", nullable: false, defaultValue: 0L),
                    ValueStr = table.Column<string>(type: "TEXT", nullable: true, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoutineData", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoutineData_Name",
                schema: "routinedata",
                table: "RoutineData",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoutineData",
                schema: "routinedata");

            migrationBuilder.AlterColumn<int>(
                name: "MaxAxisPercent",
                schema: "inputs",
                table: "Inputs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 100,
                oldClrType: typeof(double),
                oldType: "REAL",
                oldDefaultValue: 100.0);
        }
    }
}
