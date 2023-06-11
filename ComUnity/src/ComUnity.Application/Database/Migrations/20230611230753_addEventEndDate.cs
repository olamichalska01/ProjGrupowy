using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComUnity.Application.Database.Migrations
{
    /// <inheritdoc />
    public partial class addEventEndDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EventDate",
                table: "Event",
                newName: "StartDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Event",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Event_OwnerId",
                table: "Event",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_UserProfile_OwnerId",
                table: "Event",
                column: "OwnerId",
                principalTable: "UserProfile",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_UserProfile_OwnerId",
                table: "Event");

            migrationBuilder.DropIndex(
                name: "IX_Event_OwnerId",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Event");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Event",
                newName: "EventDate");
        }
    }
}
