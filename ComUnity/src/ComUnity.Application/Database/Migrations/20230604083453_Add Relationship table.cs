using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComUnity.Application.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddRelationshiptable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "UserProfile",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "Relationship",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    User1Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    User2Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RelationshipType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Relationship", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Relationship_UserProfile_User1Id",
                        column: x => x.User1Id,
                        principalTable: "UserProfile",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Relationship_UserProfile_User2Id",
                        column: x => x.User2Id,
                        principalTable: "UserProfile",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Relationship_User1Id_User2Id",
                table: "Relationship",
                columns: new[] { "User1Id", "User2Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Relationship_User2Id",
                table: "Relationship",
                column: "User2Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Relationship");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "UserProfile");
        }
    }
}
