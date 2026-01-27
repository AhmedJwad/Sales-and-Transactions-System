using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sale.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddColorAndSizeToOrderDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ColorId",
                table: "ordersDetail",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ColorName",
                table: "ordersDetail",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SizeId",
                table: "ordersDetail",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SizeName",
                table: "ordersDetail",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColorId",
                table: "ordersDetail");

            migrationBuilder.DropColumn(
                name: "ColorName",
                table: "ordersDetail");

            migrationBuilder.DropColumn(
                name: "SizeId",
                table: "ordersDetail");

            migrationBuilder.DropColumn(
                name: "SizeName",
                table: "ordersDetail");
        }
    }
}
