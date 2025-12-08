using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sale.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrencyAndProductPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cost",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DesiredProfit",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Products");

            migrationBuilder.CreateTable(
                name: "currencies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18,6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_currencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "productPrices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_productPrices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_productPrices_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_productPrices_currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_productPrices_CurrencyId",
                table: "productPrices",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_productPrices_ProductId_CurrencyId",
                table: "productPrices",
                columns: new[] { "ProductId", "CurrencyId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "productPrices");

            migrationBuilder.DropTable(
                name: "currencies");

            migrationBuilder.AddColumn<decimal>(
                name: "Cost",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DesiredProfit",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
