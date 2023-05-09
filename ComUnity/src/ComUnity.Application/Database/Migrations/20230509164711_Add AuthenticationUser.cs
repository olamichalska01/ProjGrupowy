using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComUnity.Application.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthenticationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuthenticationUser",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    HashedPassword = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    IsEmailVerified = table.Column<bool>(type: "bit", nullable: false),
                    SecurityCode = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    SecurityCodeExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthenticationUser", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthenticationUser_Email",
                table: "AuthenticationUser",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_AuthenticationUser_SecurityCode",
                table: "AuthenticationUser",
                column: "SecurityCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthenticationUser");
        }
    }
}
