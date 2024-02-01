using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Worktime.Migrations
{
    public partial class addLogTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogTime",
                table: "Pointer");

            migrationBuilder.AddColumn<DateTime>(
                name: "LogTime",
                table: "Passage",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogTime",
                table: "Passage");

            migrationBuilder.AddColumn<DateTime>(
                name: "LogTime",
                table: "Pointer",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
