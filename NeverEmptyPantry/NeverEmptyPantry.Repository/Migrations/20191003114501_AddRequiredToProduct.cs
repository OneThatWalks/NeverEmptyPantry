using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace NeverEmptyPantry.Repository.Migrations
{
    public partial class AddRequiredToProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97ee4ccb-190a-42a5-9ab5-3f2b28172f23");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Product",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Brand",
                table: "Product",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Categories",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3395ae77-69d1-4d01-a509-75732635aeba");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Product",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Brand",
                table: "Product",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Categories",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "97ee4ccb-190a-42a5-9ab5-3f2b28172f23", "543fcbd7-3e60-4262-9a0f-5b582df13808", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.UpdateData(
                table: "OfficeLocation",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDateTimeUtc",
                value: new DateTime(2019, 9, 13, 18, 18, 15, 137, DateTimeKind.Utc).AddTicks(2486));
        }
    }
}
