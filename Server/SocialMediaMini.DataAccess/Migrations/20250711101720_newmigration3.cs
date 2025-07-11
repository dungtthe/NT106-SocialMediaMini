﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialMediaMini.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class newmigration3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EncryptionPrivateKey",
                table: "Users",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EncryptionPublicKey",
                table: "Users",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EncryptionPrivateKey",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EncryptionPublicKey",
                table: "Users");
        }
    }
}
