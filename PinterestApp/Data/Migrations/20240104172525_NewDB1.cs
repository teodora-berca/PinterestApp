using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PinterestApp.Data.Migrations
{
    public partial class NewDB1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "User_name",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "profilePicture",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "User_name",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "profilePicture",
                table: "AspNetUsers");
        }
    }
}
