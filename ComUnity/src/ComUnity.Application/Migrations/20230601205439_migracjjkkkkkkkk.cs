using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComUnity.Application.Migrations
{
    /// <inheritdoc />
    public partial class migracjjkkkkkkkk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventCategoryUserProfile");

            migrationBuilder.AddColumn<Guid>(
                name: "EventCategoryId",
                table: "UserProfile",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_EventCategoryId",
                table: "UserProfile",
                column: "EventCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfile_EventCategory_EventCategoryId",
                table: "UserProfile",
                column: "EventCategoryId",
                principalTable: "EventCategory",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfile_EventCategory_EventCategoryId",
                table: "UserProfile");

            migrationBuilder.DropIndex(
                name: "IX_UserProfile_EventCategoryId",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "EventCategoryId",
                table: "UserProfile");

            migrationBuilder.CreateTable(
                name: "EventCategoryUserProfile",
                columns: table => new
                {
                    FavoriteCategoriesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsersUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventCategoryUserProfile", x => new { x.FavoriteCategoriesId, x.UsersUserId });
                    table.ForeignKey(
                        name: "FK_EventCategoryUserProfile_EventCategory_FavoriteCategoriesId",
                        column: x => x.FavoriteCategoriesId,
                        principalTable: "EventCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventCategoryUserProfile_UserProfile_UsersUserId",
                        column: x => x.UsersUserId,
                        principalTable: "UserProfile",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventCategoryUserProfile_UsersUserId",
                table: "EventCategoryUserProfile",
                column: "UsersUserId");
        }
    }
}
