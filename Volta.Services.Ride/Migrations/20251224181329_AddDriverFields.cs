using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Volta.Services.Ride.Migrations
{
    /// <inheritdoc />
    public partial class AddDriverFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DriverName",
                table: "Rides",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DriverName",
                table: "Rides");
        }
    }
}
