using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Volta.Services.Ride.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRideSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookingTime",
                table: "Rides");

            migrationBuilder.RenameColumn(
                name: "Eta",
                table: "Rides",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "DriverName",
                table: "Rides",
                newName: "PickupLocation");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Rides",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<string>(
                name: "DropoffLocation",
                table: "Rides",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PassengerName",
                table: "Rides",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DropoffLocation",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "PassengerName",
                table: "Rides");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Rides",
                newName: "Eta");

            migrationBuilder.RenameColumn(
                name: "PickupLocation",
                table: "Rides",
                newName: "DriverName");

            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "Rides",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<DateTime>(
                name: "BookingTime",
                table: "Rides",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
