using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComUnity.Application.Migrations
{
    /// <inheritdoc />
    public partial class migracjjj : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventCategoryUserProfile_EventCategory_FavoriteCategoriesEventCategoryId",
                table: "EventCategoryUserProfile");

            migrationBuilder.RenameColumn(
                name: "FavoriteCategoriesEventCategoryId",
                table: "EventCategoryUserProfile",
                newName: "FavoriteCategoriesId");

            migrationBuilder.RenameColumn(
                name: "EventCategoryId",
                table: "EventCategory",
                newName: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventCategoryUserProfile_EventCategory_FavoriteCategoriesId",
                table: "EventCategoryUserProfile",
                column: "FavoriteCategoriesId",
                principalTable: "EventCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventCategoryUserProfile_EventCategory_FavoriteCategoriesId",
                table: "EventCategoryUserProfile");

            migrationBuilder.RenameColumn(
                name: "FavoriteCategoriesId",
                table: "EventCategoryUserProfile",
                newName: "FavoriteCategoriesEventCategoryId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "EventCategory",
                newName: "EventCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventCategoryUserProfile_EventCategory_FavoriteCategoriesEventCategoryId",
                table: "EventCategoryUserProfile",
                column: "FavoriteCategoriesEventCategoryId",
                principalTable: "EventCategory",
                principalColumn: "EventCategoryId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
