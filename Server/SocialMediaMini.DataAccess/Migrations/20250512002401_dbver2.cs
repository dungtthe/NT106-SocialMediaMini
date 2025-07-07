using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialMediaMini.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class dbver2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Avatar",
                table: "ChatRooms",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "ChatRooms");
        }
    }
}
