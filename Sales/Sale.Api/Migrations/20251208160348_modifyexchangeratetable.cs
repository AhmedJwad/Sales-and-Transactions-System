using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sale.Api.Migrations
{
    /// <inheritdoc />
    public partial class modifyexchangeratetable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExchangeRate_currencies_BaseCurrencyId",
                table: "ExchangeRate");

            migrationBuilder.DropForeignKey(
                name: "FK_ExchangeRate_currencies_TargetCurrencyId",
                table: "ExchangeRate");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExchangeRate",
                table: "ExchangeRate");

            migrationBuilder.RenameTable(
                name: "ExchangeRate",
                newName: "exchangeRates");

            migrationBuilder.RenameIndex(
                name: "IX_ExchangeRate_TargetCurrencyId",
                table: "exchangeRates",
                newName: "IX_exchangeRates_TargetCurrencyId");

            migrationBuilder.RenameIndex(
                name: "IX_ExchangeRate_BaseCurrencyId",
                table: "exchangeRates",
                newName: "IX_exchangeRates_BaseCurrencyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_exchangeRates",
                table: "exchangeRates",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_exchangeRates_currencies_BaseCurrencyId",
                table: "exchangeRates",
                column: "BaseCurrencyId",
                principalTable: "currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_exchangeRates_currencies_TargetCurrencyId",
                table: "exchangeRates",
                column: "TargetCurrencyId",
                principalTable: "currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_exchangeRates_currencies_BaseCurrencyId",
                table: "exchangeRates");

            migrationBuilder.DropForeignKey(
                name: "FK_exchangeRates_currencies_TargetCurrencyId",
                table: "exchangeRates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_exchangeRates",
                table: "exchangeRates");

            migrationBuilder.RenameTable(
                name: "exchangeRates",
                newName: "ExchangeRate");

            migrationBuilder.RenameIndex(
                name: "IX_exchangeRates_TargetCurrencyId",
                table: "ExchangeRate",
                newName: "IX_ExchangeRate_TargetCurrencyId");

            migrationBuilder.RenameIndex(
                name: "IX_exchangeRates_BaseCurrencyId",
                table: "ExchangeRate",
                newName: "IX_ExchangeRate_BaseCurrencyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExchangeRate",
                table: "ExchangeRate",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExchangeRate_currencies_BaseCurrencyId",
                table: "ExchangeRate",
                column: "BaseCurrencyId",
                principalTable: "currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ExchangeRate_currencies_TargetCurrencyId",
                table: "ExchangeRate",
                column: "TargetCurrencyId",
                principalTable: "currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
