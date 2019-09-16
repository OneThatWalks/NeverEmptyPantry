using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NeverEmptyPantry.Repository.Migrations
{
    public partial class IndyOfficeSeed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3d075d96-482b-46ad-96ff-bb543721d80a");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "97ee4ccb-190a-42a5-9ab5-3f2b28172f23", "543fcbd7-3e60-4262-9a0f-5b582df13808", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.InsertData(
                table: "OfficeLocation",
                columns: new[] { "Id", "Active", "Address", "City", "Country", "CreatedDateTimeUtc", "ModifiedDateTimeUtc", "Name", "State", "Zip" },
                values: new object[] { 1, true, "9025 River Road Suite 150", "Indianapolis", "USA", new DateTime(2019, 9, 13, 18, 18, 15, 137, DateTimeKind.Utc).AddTicks(2486), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Indy Office", "IN", "46240" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97ee4ccb-190a-42a5-9ab5-3f2b28172f23");

            migrationBuilder.DeleteData(
                table: "OfficeLocation",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "OfficeLocationId", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "Title", "TwoFactorEnabled", "UserName" },
                values: new object[] { "3d075d96-482b-46ad-96ff-bb543721d80a", 0, "199f2a5a-7e87-4e83-b465-7c8dc6171ec7", null, true, null, null, false, null, null, null, null, "AQAAAAEAACcQAAAAEP8eOrQHBe8pnh+jfkkGszit1BZDOsx+mo2EVU54NDeb7d5J2YdEHnLtuQwUjEIpKQ==", null, false, null, null, false, "System" });
        }
    }
}
