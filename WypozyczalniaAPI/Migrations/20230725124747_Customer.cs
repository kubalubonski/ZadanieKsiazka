using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WypozyczalniaAPI.Migrations
{
    /// <inheritdoc />
    public partial class Customer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    customerid = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    surname = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("customer_pkey", x => x.customerid);
                });

            migrationBuilder.CreateTable(
                name: "Rentals",
                columns: table => new
                {
                    rentid = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    surname = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    rent_date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    bookid = table.Column<int>(type: "INTEGER", nullable: false),
                    Customerid = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("rental_pkey", x => x.rentid);
                    table.ForeignKey(
                        name: "FK_Rentals_Customers_Customerid",
                        column: x => x.Customerid,
                        principalTable: "Customers",
                        principalColumn: "customerid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rentals_Customerid",
                table: "Rentals",
                column: "Customerid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rentals");

            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}
