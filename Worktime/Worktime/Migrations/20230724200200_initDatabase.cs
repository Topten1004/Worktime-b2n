using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Worktime.Migrations
{
    public partial class initDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ROLE",
                table: "User",
                newName: "Role");

            migrationBuilder.RenameColumn(
                name: "LOGIN",
                table: "User",
                newName: "Login");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Role",
                table: "User",
                newName: "ROLE");

            migrationBuilder.RenameColumn(
                name: "Login",
                table: "User",
                newName: "LOGIN");
        }
    }
}
