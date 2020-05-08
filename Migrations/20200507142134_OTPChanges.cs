using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoviIDApiCore.Migrations
{
    public partial class OTPChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OtpTokens_Wallets_WalletId",
                table: "OtpTokens");

            migrationBuilder.DropIndex(
                name: "IX_OtpTokens_WalletId",
                table: "OtpTokens");

            migrationBuilder.DropColumn(
                name: "WalletId",
                table: "OtpTokens");

            migrationBuilder.AddColumn<string>(
                name: "SessionId",
                table: "OtpTokens",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "OtpTokens");

            migrationBuilder.AddColumn<Guid>(
                name: "WalletId",
                table: "OtpTokens",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OtpTokens_WalletId",
                table: "OtpTokens",
                column: "WalletId");

            migrationBuilder.AddForeignKey(
                name: "FK_OtpTokens_Wallets_WalletId",
                table: "OtpTokens",
                column: "WalletId",
                principalTable: "Wallets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
