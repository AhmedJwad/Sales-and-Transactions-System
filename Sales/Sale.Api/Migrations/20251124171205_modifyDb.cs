using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sale.Api.Migrations
{
    /// <inheritdoc />
    public partial class modifyDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_productsubCategories_subcategoryTranslations_SubcategoryTranslationId",
                table: "productsubCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_subcategoryTranslations_subcategories_SubcategoryId",
                table: "subcategoryTranslations");

            migrationBuilder.DropIndex(
                name: "IX_productsubCategories_SubcategoryTranslationId",
                table: "productsubCategories");

            migrationBuilder.DropColumn(
                name: "SubcategoryTranslationId",
                table: "productsubCategories");

            migrationBuilder.AddForeignKey(
                name: "FK_subcategoryTranslations_subcategories_SubcategoryId",
                table: "subcategoryTranslations",
                column: "SubcategoryId",
                principalTable: "subcategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_subcategoryTranslations_subcategories_SubcategoryId",
                table: "subcategoryTranslations");

            migrationBuilder.AddColumn<int>(
                name: "SubcategoryTranslationId",
                table: "productsubCategories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_productsubCategories_SubcategoryTranslationId",
                table: "productsubCategories",
                column: "SubcategoryTranslationId");

            migrationBuilder.AddForeignKey(
                name: "FK_productsubCategories_subcategoryTranslations_SubcategoryTranslationId",
                table: "productsubCategories",
                column: "SubcategoryTranslationId",
                principalTable: "subcategoryTranslations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_subcategoryTranslations_subcategories_SubcategoryId",
                table: "subcategoryTranslations",
                column: "SubcategoryId",
                principalTable: "subcategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
