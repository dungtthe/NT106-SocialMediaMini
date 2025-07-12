using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialMediaMini.DataAccess.Migrations
{
    public partial class AddAvatarAndStatusToUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Avatar",
                table: "Users",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                defaultValue: "/Resources/Images/meolag.jpg");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Users",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                defaultValue: "Offline");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Users");
        }
    }
}