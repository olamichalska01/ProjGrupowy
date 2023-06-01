using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComUnity.Application.Migrations
{
    /// <inheritdoc />
    public partial class migracjjkkkkkkkkk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_EventCategory_EventCategoryId1",
                table: "Event");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfile_EventCategory_EventCategoryId",
                table: "UserProfile");

            migrationBuilder.DropIndex(
                name: "IX_UserProfile_EventCategoryId",
                table: "UserProfile");

            migrationBuilder.DropIndex(
                name: "IX_Event_EventCategoryId1",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "EventCategoryId",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "EventCategoryId1",
                table: "Event");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EventCategoryId",
                table: "UserProfile",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EventCategoryId1",
                table: "Event",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_EventCategoryId",
                table: "UserProfile",
                column: "EventCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_EventCategoryId1",
                table: "Event",
                column: "EventCategoryId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_EventCategory_EventCategoryId1",
                table: "Event",
                column: "EventCategoryId1",
                principalTable: "EventCategory",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfile_EventCategory_EventCategoryId",
                table: "UserProfile",
                column: "EventCategoryId",
                principalTable: "EventCategory",
                principalColumn: "Id");
        }
    }
}
