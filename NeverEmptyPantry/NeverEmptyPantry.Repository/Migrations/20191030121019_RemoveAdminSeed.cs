using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NeverEmptyPantry.Repository.Migrations
{
    public partial class RemoveAdminSeed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3395ae77-69d1-4d01-a509-75732635aeba");

            migrationBuilder.UpdateData(
                table: "OfficeLocation",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDateTimeUtc",
                value: new DateTime(2019, 10, 30, 12, 10, 18, 844, DateTimeKind.Utc).AddTicks(8603));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "3395ae77-69d1-4d01-a509-75732635aeba", "faf6be9e-cfff-469e-b5f1-0f7a70591225", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.UpdateData(
                table: "OfficeLocation",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDateTimeUtc",
                value: new DateTime(2019, 10, 3, 11, 45, 0, 833, DateTimeKind.Utc).AddTicks(4833));
        }
    }
}
