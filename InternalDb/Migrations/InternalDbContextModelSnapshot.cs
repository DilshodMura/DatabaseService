﻿// <auto-generated />
using System;
using InternalDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace InternalDb.Migrations
{
    [DbContext(typeof(InternalDbContext))]
    partial class InternalDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("InternalDb.Entities.InternalCity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CountryId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CountryId");

                    b.ToTable("InternalCities");
                });

            modelBuilder.Entity("InternalDb.Entities.InternalCountry", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("InternalCountries");
                });

            modelBuilder.Entity("InternalDb.Entities.InternalOffice", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CityId")
                        .HasColumnType("int");

                    b.Property<int?>("InternalCountryId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CityId");

                    b.HasIndex("InternalCountryId");

                    b.ToTable("InternalOffices");
                });

            modelBuilder.Entity("InternalDb.Entities.InternalCity", b =>
                {
                    b.HasOne("InternalDb.Entities.InternalCountry", "Country")
                        .WithMany("Cities")
                        .HasForeignKey("CountryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Country");
                });

            modelBuilder.Entity("InternalDb.Entities.InternalOffice", b =>
                {
                    b.HasOne("InternalDb.Entities.InternalCity", "City")
                        .WithMany("Offices")
                        .HasForeignKey("CityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("InternalDb.Entities.InternalCountry", null)
                        .WithMany("Offices")
                        .HasForeignKey("InternalCountryId");

                    b.Navigation("City");
                });

            modelBuilder.Entity("InternalDb.Entities.InternalCity", b =>
                {
                    b.Navigation("Offices");
                });

            modelBuilder.Entity("InternalDb.Entities.InternalCountry", b =>
                {
                    b.Navigation("Cities");

                    b.Navigation("Offices");
                });
#pragma warning restore 612, 618
        }
    }
}