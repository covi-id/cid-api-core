﻿// <auto-generated />
using System;
using CoviIDApiCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CoviIDApiCore.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20200601083733_PrivacyPreserving_Restructure")]
    partial class PrivacyPreserving_Restructure
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CoviIDApiCore.Models.Database.Organisation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("Name");

                    b.Property<string>("Payload");

                    b.HasKey("Id");

                    b.ToTable("Organisations");
                });

            modelBuilder.Entity("CoviIDApiCore.Models.Database.OrganisationAccessLog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<Guid?>("OrganisationId");

                    b.Property<string>("ScanType")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("OrganisationId");

                    b.ToTable("OrganisationAccessLogs");
                });

            modelBuilder.Entity("CoviIDApiCore.Models.Database.OtpToken", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Code");

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<DateTime?>("ExpireAt");

                    b.Property<string>("MobileNumber");

                    b.Property<bool>("isUsed");

                    b.HasKey("Id");

                    b.ToTable("OtpTokens");
                });

            modelBuilder.Entity("CoviIDApiCore.Models.Database.Session", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<DateTime?>("ExpireAt");

                    b.Property<Guid?>("WalletId");

                    b.Property<bool>("isUsed");

                    b.HasKey("Id");

                    b.HasIndex("WalletId");

                    b.ToTable("Sessions");
                });

            modelBuilder.Entity("CoviIDApiCore.Models.Database.Wallet", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<DateTime?>("MobileNumberVerifiedAt");

                    b.HasKey("Id");

                    b.ToTable("Wallets");
                });

            modelBuilder.Entity("CoviIDApiCore.Models.Database.WalletDetail", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("FirstName");

                    b.Property<bool>("HasConsent");

                    b.Property<string>("LastName");

                    b.Property<string>("MobileNumber");

                    b.Property<string>("PhotoReference");

                    b.Property<Guid?>("WalletId");

                    b.Property<bool>("isMyMobileNumber");

                    b.HasKey("Id");

                    b.HasIndex("WalletId");

                    b.ToTable("WalletDetails");
                });

            modelBuilder.Entity("CoviIDApiCore.Models.Database.WalletLocationReceipt", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<decimal>("Latitude")
                        .HasColumnType("decimal(12,8)");

                    b.Property<decimal>("Longitude")
                        .HasColumnType("decimal(12,8)");

                    b.Property<string>("ScanType")
                        .IsRequired();

                    b.Property<Guid?>("WalletId");

                    b.HasKey("Id");

                    b.HasIndex("WalletId");

                    b.ToTable("WalletLocationReceipts");
                });

            modelBuilder.Entity("CoviIDApiCore.Models.Database.WalletTestResult", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<bool>("HasConsent");

                    b.Property<bool>("HasReceivedResults");

                    b.Property<DateTime?>("IssuedAt");

                    b.Property<string>("Laboratory")
                        .IsRequired();

                    b.Property<string>("LaboratoryStatus")
                        .IsRequired();

                    b.Property<DateTime?>("PermissionGrantedAt");

                    b.Property<string>("ReferenceNumber");

                    b.Property<string>("ResultStatus")
                        .IsRequired();

                    b.Property<string>("TestType")
                        .IsRequired();

                    b.Property<DateTime?>("TestedAt");

                    b.Property<Guid?>("WalletId");

                    b.HasKey("Id");

                    b.HasIndex("WalletId");

                    b.ToTable("WalletTestResults");
                });

            modelBuilder.Entity("CoviIDApiCore.Models.Database.OrganisationAccessLog", b =>
                {
                    b.HasOne("CoviIDApiCore.Models.Database.Organisation", "Organisation")
                        .WithMany("AccessLogs")
                        .HasForeignKey("OrganisationId");
                });

            modelBuilder.Entity("CoviIDApiCore.Models.Database.Session", b =>
                {
                    b.HasOne("CoviIDApiCore.Models.Database.Wallet", "Wallet")
                        .WithMany()
                        .HasForeignKey("WalletId");
                });

            modelBuilder.Entity("CoviIDApiCore.Models.Database.WalletDetail", b =>
                {
                    b.HasOne("CoviIDApiCore.Models.Database.Wallet", "Wallet")
                        .WithMany()
                        .HasForeignKey("WalletId");
                });

            modelBuilder.Entity("CoviIDApiCore.Models.Database.WalletLocationReceipt", b =>
                {
                    b.HasOne("CoviIDApiCore.Models.Database.Wallet", "Wallet")
                        .WithMany()
                        .HasForeignKey("WalletId");
                });

            modelBuilder.Entity("CoviIDApiCore.Models.Database.WalletTestResult", b =>
                {
                    b.HasOne("CoviIDApiCore.Models.Database.Wallet", "Wallet")
                        .WithMany()
                        .HasForeignKey("WalletId");
                });
#pragma warning restore 612, 618
        }
    }
}
