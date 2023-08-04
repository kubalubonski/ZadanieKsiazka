﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WypozyczalniaAPI.Entities;

#nullable disable

namespace WypozyczalniaAPI.Migrations
{
    [DbContext(typeof(RentalContext))]
    partial class RentalContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.9");

            modelBuilder.Entity("WypozyczalniaAPI.Entities.Customer", b =>
                {
                    b.Property<int>("Customerid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("customerid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT")
                        .HasColumnName("name");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT")
                        .HasColumnName("surname");

                    b.HasKey("Customerid")
                        .HasName("customer_pkey");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("WypozyczalniaAPI.Entities.Rental", b =>
                {
                    b.Property<int>("Rentid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("rentid");

                    b.Property<int>("Bookid")
                        .HasColumnType("INTEGER")
                        .HasColumnName("bookid");

                    b.Property<int>("Customerid")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT")
                        .HasColumnName("name");

                    b.Property<DateTime>("RentDate")
                        .HasColumnType("TEXT")
                        .HasColumnName("rent_date");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT")
                        .HasColumnName("surname");

                    b.HasKey("Rentid")
                        .HasName("rental_pkey");

                    b.HasIndex("Customerid");

                    b.ToTable("Rentals");
                });

            modelBuilder.Entity("WypozyczalniaAPI.Entities.Rental", b =>
                {
                    b.HasOne("WypozyczalniaAPI.Entities.Customer", "Customer")
                        .WithMany("Rentals")
                        .HasForeignKey("Customerid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("WypozyczalniaAPI.Entities.Customer", b =>
                {
                    b.Navigation("Rentals");
                });
#pragma warning restore 612, 618
        }
    }
}
