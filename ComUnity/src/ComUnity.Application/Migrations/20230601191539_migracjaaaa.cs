using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComUnity.Application.Migrations
{
    /// <inheritdoc />
    public partial class migracjaaaa : Migration
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

            migrationBuilder.CreateTable(
                name: "EventCategory",
                columns: table => new
                {
                    EventCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventCategory", x => x.EventCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "UserProfile",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfile", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Event",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaxAmountOfPeople = table.Column<int>(type: "int", nullable: false),
                    Place = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Cost = table.Column<double>(type: "float", nullable: false),
                    MinAge = table.Column<int>(type: "int", nullable: false),
                    EventCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventCategoryId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Event_EventCategory_EventCategoryId",
                        column: x => x.EventCategoryId,
                        principalTable: "EventCategory",
                        principalColumn: "EventCategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Event_EventCategory_EventCategoryId1",
                        column: x => x.EventCategoryId1,
                        principalTable: "EventCategory",
                        principalColumn: "EventCategoryId");
                });

            migrationBuilder.CreateTable(
                name: "EventCategoryUserProfile",
                columns: table => new
                {
                    FavoriteCategoriesEventCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsersUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventCategoryUserProfile", x => new { x.FavoriteCategoriesEventCategoryId, x.UsersUserId });
                    table.ForeignKey(
                        name: "FK_EventCategoryUserProfile_EventCategory_FavoriteCategoriesEventCategoryId",
                        column: x => x.FavoriteCategoriesEventCategoryId,
                        principalTable: "EventCategory",
                        principalColumn: "EventCategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventCategoryUserProfile_UserProfile_UsersUserId",
                        column: x => x.UsersUserId,
                        principalTable: "UserProfile",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserFavoriteEventCategory",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFavoriteEventCategory", x => new { x.UserId, x.EventCategoryId });
                    table.ForeignKey(
                        name: "FK_UserFavoriteEventCategory_EventCategory_EventCategoryId",
                        column: x => x.EventCategoryId,
                        principalTable: "EventCategory",
                        principalColumn: "EventCategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFavoriteEventCategory_UserProfile_UserId",
                        column: x => x.UserId,
                        principalTable: "UserProfile",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthenticationUser_Email",
                table: "AuthenticationUser",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_AuthenticationUser_SecurityCode",
                table: "AuthenticationUser",
                column: "SecurityCode");

            migrationBuilder.CreateIndex(
                name: "IX_Event_EventCategoryId",
                table: "Event",
                column: "EventCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_EventCategoryId1",
                table: "Event",
                column: "EventCategoryId1");

            migrationBuilder.CreateIndex(
                name: "IX_EventCategoryUserProfile_UsersUserId",
                table: "EventCategoryUserProfile",
                column: "UsersUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFavoriteEventCategory_EventCategoryId",
                table: "UserFavoriteEventCategory",
                column: "EventCategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthenticationUser");

            migrationBuilder.DropTable(
                name: "Event");

            migrationBuilder.DropTable(
                name: "EventCategoryUserProfile");

            migrationBuilder.DropTable(
                name: "UserFavoriteEventCategory");

            migrationBuilder.DropTable(
                name: "EventCategory");

            migrationBuilder.DropTable(
                name: "UserProfile");
        }
    }
}
