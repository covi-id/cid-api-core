using Microsoft.EntityFrameworkCore.Migrations;

namespace CoviIDApiCore.Migrations
{
    public partial class ConversionofTestType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TestType",
                table: "WalletTestResults",
                nullable: false,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "TestType",
                table: "WalletTestResults",
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
