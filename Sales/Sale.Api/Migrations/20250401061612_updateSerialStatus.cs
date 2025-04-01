using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sale.Api.Migrations
{
    /// <inheritdoc />
    public partial class updateSerialStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "serialNumbers");

            migrationBuilder.AddColumn<int>(
                name: "SerialStatus",
                table: "serialNumbers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SerialStatus",
                table: "serialNumbers");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "serialNumbers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
