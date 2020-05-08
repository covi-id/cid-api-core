using Microsoft.EntityFrameworkCore.Migrations;

namespace CoviIDApiCore.Migrations
{
    public partial class RemovedSessionId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "OtpTokens");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SessionId",
                table: "Wallets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SessionId",
                table: "OtpTokens",
                nullable: true);
        }
    }
}
