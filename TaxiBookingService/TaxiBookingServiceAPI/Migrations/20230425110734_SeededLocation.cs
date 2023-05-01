using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaxiBookingService.Host.Migrations
{
    public partial class SeededLocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Areas",
                columns: new[] { "Id", "CityId", "Deleted", "Name" },
                values: new object[,]
                {
                    { 6, 3, false, "Vaishali Nagar" },
                    { 7, 3, false, "Sanganer" }
                });

            migrationBuilder.UpdateData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 2,
                column: "StreetName",
                value: "InTimeTec RIICO Industrial Area");

            migrationBuilder.InsertData(
                table: "Locations",
                columns: new[] { "Id", "AreaId", "Deleted", "Latitude", "Longitude", "StreetName" },
                values: new object[] { 7, 1, false, 26.954195m, 75.784245m, "RPA Road Sector 4" });

            migrationBuilder.InsertData(
                table: "Locations",
                columns: new[] { "Id", "AreaId", "Deleted", "Latitude", "Longitude", "StreetName" },
                values: new object[,]
                {
                    { 6, 6, false, 26.914442m, 75.738313m, "Vaishali Marg Block C" },
                    { 8, 6, false, 26.906310m, 75.735119m, "Gandhi Path Block B" },
                    { 9, 7, false, 26.829775m, 75.805197m, "Airport Road Surajpura" },
                    { 10, 6, false, 26.912616m, 75.743459m, "Amrapali Marg E block" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Areas",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Areas",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.UpdateData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 2,
                column: "StreetName",
                value: "RIICO Industrial Area");
        }
    }
}
