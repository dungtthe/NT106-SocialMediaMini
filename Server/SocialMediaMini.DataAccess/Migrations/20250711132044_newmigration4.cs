using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialMediaMini.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class newmigration4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EncryptionPrivateKey",
                table: "Users",
                newName: "IV");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IV",
                table: "Users",
                newName: "EncryptionPrivateKey");
        }
    }
}
