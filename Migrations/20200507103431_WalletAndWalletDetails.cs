using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoviIDApiCore.Migrations
{
    public partial class WalletAndWalletDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WalletIdentifier",
                table: "Wallets",
                newName: "MobileNumberReference");

            migrationBuilder.AddColumn<string>(
                name: "MobileNumber",
                table: "Wallets",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MobileNumberVerifiedAt",
                table: "Wallets",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "WalletDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    WalletId = table.Column<Guid>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    PhotoUrl = table.Column<string>(nullable: true),
                    IdType = table.Column<string>(nullable: false),
                    IdValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WalletDetails_Wallets_WalletId",
                        column: x => x.WalletId,
                        principalTable: "Wallets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WalletTestResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    WalletId = table.Column<Guid>(nullable: true),
                    LaboratoryStatus = table.Column<string>(nullable: false),
                    ResultStatus = table.Column<string>(nullable: false),
                    Laboratory = table.Column<string>(nullable: false),
                    ReferenceNumber = table.Column<string>(nullable: true),
                    TestedAt = table.Column<DateTime>(nullable: false),
                    IssuedAt = table.Column<DateTime>(nullable: false),
                    PermissionGrantedAt = table.Column<DateTime>(nullable: false),
                    HasConsent = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletTestResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WalletTestResults_Wallets_WalletId",
                        column: x => x.WalletId,
                        principalTable: "Wallets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WalletDetails_WalletId",
                table: "WalletDetails",
                column: "WalletId");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTestResults_WalletId",
                table: "WalletTestResults",
                column: "WalletId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WalletDetails");

            migrationBuilder.DropTable(
                name: "WalletTestResults");

            migrationBuilder.DropColumn(
                name: "MobileNumber",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "MobileNumberVerifiedAt",
                table: "Wallets");

            migrationBuilder.RenameColumn(
                name: "MobileNumberReference",
                table: "Wallets",
                newName: "WalletIdentifier");
        }
    }
}
