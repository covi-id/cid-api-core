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
    [Migration("20200423141130_dbo_RenameTokensTable")]
    partial class dbo_RenameTokensTable
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

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("Name");

                    b.Property<string>("Payload");

                    b.HasKey("Id");

                    b.ToTable("Organisations");
                });

            modelBuilder.Entity("CoviIDApiCore.Models.Database.OrganisationCounter", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Balance");

                    b.Property<DateTime>("Date");

                    b.Property<string>("DeviceIdentifier");

                    b.Property<int>("Movement");

                    b.Property<Guid?>("OrganisationId");

                    b.HasKey("Id");

                    b.HasIndex("OrganisationId");

                    b.ToTable("OrganisationCounters");
                });

            modelBuilder.Entity("CoviIDApiCore.Models.Database.OtpToken", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Code");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<DateTime>("ExpireAt");

                    b.Property<string>("MobileNumber");

                    b.Property<Guid?>("WalletId");

                    b.Property<bool>("isUsed");

                    b.HasKey("Id");

                    b.HasIndex("WalletId");

                    b.ToTable("OtpTokens");
                });

            modelBuilder.Entity("CoviIDApiCore.Models.Database.Wallet", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("WalletIdentifier");

                    b.HasKey("Id");

                    b.ToTable("Wallets");
                });

            modelBuilder.Entity("CoviIDApiCore.Models.Database.OrganisationCounter", b =>
                {
                    b.HasOne("CoviIDApiCore.Models.Database.Organisation", "Organisation")
                        .WithMany("Counter")
                        .HasForeignKey("OrganisationId");
                });

            modelBuilder.Entity("CoviIDApiCore.Models.Database.OtpToken", b =>
                {
                    b.HasOne("CoviIDApiCore.Models.Database.Wallet", "Wallet")
                        .WithMany("Tokens")
                        .HasForeignKey("WalletId");
                });
#pragma warning restore 612, 618
        }
    }
}
