using System;
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoviIDApiCore.Migrations
{
    public partial class PrivacyPreserving_Restructure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
              new FileInfo("20200601083733_PrivacyPreserving_Restructure_Before_UP.sql")
                  .OpenText()
                  .ReadToEnd()
                  );

            migrationBuilder.DropForeignKey(
                name: "FK_OrganisationAccessLogs_Wallets_WalletId",
                table: "OrganisationAccessLogs");

            migrationBuilder.DropTable(
                name: "CovidTests");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationAccessLogs_WalletId",
                table: "OrganisationAccessLogs");

            migrationBuilder.DropColumn(
                name: "MobileNumber",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "MobileNumberReference",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "OrganisationAccessLogs");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "OrganisationAccessLogs");

            migrationBuilder.DropColumn(
                name: "WalletId",
                table: "OrganisationAccessLogs");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "WalletTestResults",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasReceivedResults",
                table: "WalletTestResults",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "WalletDetails",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasConsent",
                table: "WalletDetails",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MobileNumber",
                table: "WalletDetails",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isMyMobileNumber",
                table: "WalletDetails",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "WalletLocationReceipts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    WalletId = table.Column<Guid>(nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(12,8)", nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(12,8)", nullable: false),
                    ScanType = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletLocationReceipts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WalletLocationReceipts_Wallets_WalletId",
                        column: x => x.WalletId,
                        principalTable: "Wallets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WalletLocationReceipts_WalletId",
                table: "WalletLocationReceipts",
                column: "WalletId");

            migrationBuilder.Sql(
              new FileInfo("20200601083733_PrivacyPreserving_Restructure_After_UP.sql")
                  .OpenText()
                  .ReadToEnd()
                  );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql(
           new FileInfo("20200601083733_PrivacyPreserving_Restructure_Before_DOWN.sql")
               .OpenText()
               .ReadToEnd()
               );

            migrationBuilder.DropTable(
                name: "WalletLocationReceipts");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "WalletTestResults");

            migrationBuilder.DropColumn(
                name: "HasReceivedResults",
                table: "WalletTestResults");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "WalletDetails");

            migrationBuilder.DropColumn(
                name: "HasConsent",
                table: "WalletDetails");

            migrationBuilder.DropColumn(
                name: "MobileNumber",
                table: "WalletDetails");

            migrationBuilder.DropColumn(
                name: "isMyMobileNumber",
                table: "WalletDetails");

            migrationBuilder.AddColumn<string>(
                name: "MobileNumber",
                table: "Wallets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MobileNumberReference",
                table: "Wallets",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "OrganisationAccessLogs",
                type: "decimal(12,8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "OrganisationAccessLogs",
                type: "decimal(12,8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "WalletId",
                table: "OrganisationAccessLogs",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CovidTests",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CovidStatus = table.Column<string>(nullable: false),
                    CredentialIndicator = table.Column<string>(nullable: false),
                    DateTested = table.Column<DateTime>(nullable: false),
                    HasConsent = table.Column<bool>(nullable: false),
                    Laboratory = table.Column<string>(nullable: false),
                    PermissionGrantedAt = table.Column<DateTime>(nullable: false),
                    ReferenceNumber = table.Column<string>(nullable: true),
                    WalletId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CovidTests", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationAccessLogs_WalletId",
                table: "OrganisationAccessLogs",
                column: "WalletId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganisationAccessLogs_Wallets_WalletId",
                table: "OrganisationAccessLogs",
                column: "WalletId",
                principalTable: "Wallets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql(
           new FileInfo("20200601083733_PrivacyPreserving_Restructure_After_DOWN.sql")
               .OpenText()
               .ReadToEnd()
               );
        }
    }
}
