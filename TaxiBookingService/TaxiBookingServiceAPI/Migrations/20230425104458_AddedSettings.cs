using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaxiBookingService.Host.Migrations
{
    public partial class AddedSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "Id", "SettingName", "Value" },
                values: new object[] { 2, "DriverRange", 2.5m });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
