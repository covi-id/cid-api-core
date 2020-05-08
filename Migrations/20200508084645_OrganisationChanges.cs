using System;
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoviIDApiCore.Migrations
{
    public partial class OrganisationChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                new FileInfo("Migrations/20200508084645_OrganisationChanges_Before_UP.sql")
                    .OpenText()
                    .ReadToEnd()
            );

            migrationBuilder.DropTable(
                name: "OrganisationCounters");

            migrationBuilder.CreateTable(
                name: "OrganisationAccessLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    OrganisationId = table.Column<Guid>(nullable: true),
                    WalletId = table.Column<Guid>(nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(12,8)", nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(12,8)", nullable: false),
                    ScanType = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationAccessLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganisationAccessLogs_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrganisationAccessLogs_Wallets_WalletId",
                        column: x => x.WalletId,
                        principalTable: "Wallets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationAccessLogs_OrganisationId",
                table: "OrganisationAccessLogs",
                column: "OrganisationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationAccessLogs_WalletId",
                table: "OrganisationAccessLogs",
                column: "WalletId");

            migrationBuilder.Sql(
                new FileInfo("Migrations/20200508084645_OrganisationChanges_After_UP.sql")
                    .OpenText()
                    .ReadToEnd()
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                new FileInfo("Migrations/20200508084645_OrganisationChanges_Before_DOWN.sql")
                    .OpenText()
                    .ReadToEnd()
            );

            migrationBuilder.DropTable(
                name: "OrganisationAccessLogs");

            migrationBuilder.CreateTable(
                name: "OrganisationCounters",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Balance = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    DeviceIdentifier = table.Column<string>(nullable: true),
                    Movement = table.Column<int>(nullable: false),
                    OrganisationId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationCounters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganisationCounters_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationCounters_OrganisationId",
                table: "OrganisationCounters",
                column: "OrganisationId");

            migrationBuilder.Sql(
                new FileInfo("Migrations/20200508084645_OrganisationChanges_After_DOWN.sql")
                    .OpenText()
                    .ReadToEnd()
            );
        }
    }
}
