using Microsoft.EntityFrameworkCore.Migrations;

namespace GestionEmploi.API.Migrations
{
    public partial class AddPublicId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "interests",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "Photos",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "interests",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "Photos");
        }
    }
}
