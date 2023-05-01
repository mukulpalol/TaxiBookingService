using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaxiBookingService.Host.Migrations
{
    public partial class UpdatedRide : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EstimatedDropTime",
                table: "Rides",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EstimatedPickUpTime",
                table: "Rides",
                type: "datetime",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstimatedDropTime",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "EstimatedPickUpTime",
                table: "Rides");
        }
    }
}
