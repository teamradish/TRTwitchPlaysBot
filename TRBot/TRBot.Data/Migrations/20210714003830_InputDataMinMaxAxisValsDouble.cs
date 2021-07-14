using Microsoft.EntityFrameworkCore.Migrations;

namespace TRBot.Data.Migrations
{
    public partial class InputDataMinMaxAxisValsDouble : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "MinAxisVal",
                schema: "inputs",
                table: "Inputs",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<double>(
                name: "MaxAxisVal",
                schema: "inputs",
                table: "Inputs",
                type: "REAL",
                nullable: false,
                defaultValue: 1.0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldDefaultValue: 1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "MinAxisVal",
                schema: "inputs",
                table: "Inputs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(double),
                oldType: "REAL",
                oldDefaultValue: 0.0);

            migrationBuilder.AlterColumn<int>(
                name: "MaxAxisVal",
                schema: "inputs",
                table: "Inputs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(double),
                oldType: "REAL",
                oldDefaultValue: 1.0);
        }
    }
}
