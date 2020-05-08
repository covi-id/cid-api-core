using Microsoft.EntityFrameworkCore.Migrations;

namespace CoviIDApiCore.Migrations
{
    public partial class SessionIdToWallet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SessionId",
                table: "Wallets",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "Wallets");
        }
    }
}
