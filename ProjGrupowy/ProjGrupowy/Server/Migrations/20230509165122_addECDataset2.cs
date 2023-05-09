using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjGrupowy.Server.Migrations
{
    /// <inheritdoc />
    public partial class addECDataset2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_EventsCategories_EventCategoryCategoryName",
                table: "Events");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventsCategories",
                table: "EventsCategories");

            migrationBuilder.RenameTable(
                name: "EventsCategories",
                newName: "EventCategories");

            migrationBuilder.AlterColumn<string>(
                name: "EventCategoryCategoryName",
                table: "Events",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventCategories",
                table: "EventCategories",
                column: "CategoryName");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_EventCategories_EventCategoryCategoryName",
                table: "Events",
                column: "EventCategoryCategoryName",
                principalTable: "EventCategories",
                principalColumn: "CategoryName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_EventCategories_EventCategoryCategoryName",
                table: "Events");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventCategories",
                table: "EventCategories");

            migrationBuilder.RenameTable(
                name: "EventCategories",
                newName: "EventsCategories");

            migrationBuilder.AlterColumn<string>(
                name: "EventCategoryCategoryName",
                table: "Events",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

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
    }
}
