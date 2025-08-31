using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sale.Api.Migrations
{
    /// <inheritdoc />
    public partial class modifyproductcolorimages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductImages_colors_ColorId",
                table: "ProductImages");

            migrationBuilder.DropIndex(
                name: "IX_ProductImages_ColorId",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "ColorId",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "productcolorId",
                table: "ProductImages");

            migrationBuilder.CreateTable(
                name: "productColorImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ColorId = table.Column<int>(type: "int", nullable: false),
                    ProductImageId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_productColorImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_productColorImages_ProductImages_ProductImageId",
                        column: x => x.ProductImageId,
                        principalTable: "ProductImages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_productColorImages_colors_ColorId",
                        column: x => x.ColorId,
                        principalTable: "colors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_productColorImages_ColorId",
                table: "productColorImages",
                column: "ColorId");

            migrationBuilder.CreateIndex(
                name: "IX_productColorImages_ProductImageId",
                table: "productColorImages",
                column: "ProductImageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "productColorImages");

            migrationBuilder.AddColumn<int>(
                name: "ColorId",
                table: "ProductImages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "productcolorId",
                table: "ProductImages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ColorId",
                table: "ProductImages",
                column: "ColorId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImages_colors_ColorId",
                table: "ProductImages",
                column: "ColorId",
                principalTable: "colors",
                principalColumn: "Id");
        }
    }
}
