using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComUnity.Application.Database.Migrations
{
    /// <inheritdoc />
    public partial class addedEventUserRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventUserProfile",
                columns: table => new
                {
                    ParticipantsUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserEventsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventUserProfile", x => new { x.ParticipantsUserId, x.UserEventsId });
                    table.ForeignKey(
                        name: "FK_EventUserProfile_Event_UserEventsId",
                        column: x => x.UserEventsId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventUserProfile_UserProfile_ParticipantsUserId",
                        column: x => x.ParticipantsUserId,
                        principalTable: "UserProfile",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventUserProfile_UserEventsId",
                table: "EventUserProfile",
                column: "UserEventsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventUserProfile");
        }
    }
}
