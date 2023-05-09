using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjGrupowy.Server.Migrations
{
    /// <inheritdoc />
    public partial class addECDataset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_EventCategory_eventCategoryCategoryName",
                table: "Events");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventCategory",
                table: "EventCategory");

            migrationBuilder.RenameTable(
                name: "EventCategory",
                newName: "EventsCategories");

            migrationBuilder.RenameColumn(
                name: "eventCategoryCategoryName",
                table: "Events",
                newName: "EventCategoryCategoryName");

            migrationBuilder.RenameIndex(
                name: "IX_Events_eventCategoryCategoryName",
                table: "Events",
                newName: "IX_Events_EventCategoryCategoryName");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventsCategories",
                table: "EventsCategories",
                column: "CategoryName");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_EventsCategories_EventCategoryCategoryName",
                table: "Events",
                column: "EventCategoryCategoryName",
                principalTable: "EventsCategories",
                principalColumn: "CategoryName",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_EventsCategories_EventCategoryCategoryName",
                table: "Events");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventsCategories",
                table: "EventsCategories");

            migrationBuilder.RenameTable(
                name: "EventsCategories",
                newName: "EventCategory");

            migrationBuilder.RenameColumn(
                name: "EventCategoryCategoryName",
                table: "Events",
                newName: "eventCategoryCategoryName");

            migrationBuilder.RenameIndex(
                name: "IX_Events_EventCategoryCategoryName",
                table: "Events",
                newName: "IX_Events_eventCategoryCategoryName");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventCategory",
                table: "EventCategory",
                column: "CategoryName");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_EventCategory_eventCategoryCategoryName",
                table: "Events",
                column: "eventCategoryCategoryName",
                principalTable: "EventCategory",
                principalColumn: "CategoryName",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
