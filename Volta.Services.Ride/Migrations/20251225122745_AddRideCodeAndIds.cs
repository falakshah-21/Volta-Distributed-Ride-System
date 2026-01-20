using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Volta.Services.Ride.Migrations
{
    /// <inheritdoc />
    public partial class AddRideCodeAndIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DriverId",
                table: "Rides",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PassengerId",
                table: "Rides",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RideCode",
                table: "Rides",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DriverId",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "PassengerId",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "RideCode",
                table: "Rides");
        }
    }
}
