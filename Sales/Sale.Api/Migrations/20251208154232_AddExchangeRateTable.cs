using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sale.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddExchangeRateTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExchangeRate",
                table: "currencies");

            migrationBuilder.CreateTable(
                name: "ExchangeRate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BaseCurrencyId = table.Column<int>(type: "int", nullable: false),
                    TargetCurrencyId = table.Column<int>(type: "int", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeRate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExchangeRate_currencies_BaseCurrencyId",
                        column: x => x.BaseCurrencyId,
                        principalTable: "currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExchangeRate_currencies_TargetCurrencyId",
                        column: x => x.TargetCurrencyId,
                        principalTable: "currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRate_BaseCurrencyId",
                table: "ExchangeRate",
                column: "BaseCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRate_TargetCurrencyId",
                table: "ExchangeRate",
                column: "TargetCurrencyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExchangeRate");

            migrationBuilder.AddColumn<decimal>(
                name: "ExchangeRate",
                table: "currencies",
                type: "decimal(18,6)",
                nullable: true);
        }
    }
}
