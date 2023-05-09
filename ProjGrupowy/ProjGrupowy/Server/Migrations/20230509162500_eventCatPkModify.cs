using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjGrupowy.Server.Migrations
{
    /// <inheritdoc />
    public partial class eventCatPkModify : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_EventCategory_eventCategoryCategoryId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_eventCategoryCategoryId",
                table: "Events");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventCategory",
                table: "EventCategory");

            migrationBuilder.DropColumn(
                name: "eventCategoryCategoryId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "EventCategory");

            migrationBuilder.AddColumn<string>(
                name: "eventCategoryCategoryName",
                table: "Events",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "CategoryName",
                table: "EventCategory",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventCategory",
                table: "EventCategory",
                column: "CategoryName");

            migrationBuilder.CreateIndex(
                name: "IX_Events_eventCategoryCategoryName",
                table: "Events",
                column: "eventCategoryCategoryName");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_EventCategory_eventCategoryCategoryName",
                table: "Events",
                column: "eventCategoryCategoryName",
                principalTable: "EventCategory",
                principalColumn: "CategoryName",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_EventCategory_eventCategoryCategoryName",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_eventCategoryCategoryName",
                table: "Events");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventCategory",
                table: "EventCategory");

            migrationBuilder.DropColumn(
                name: "eventCategoryCategoryName",
                table: "Events");

            migrationBuilder.AddColumn<int>(
                name: "eventCategoryCategoryId",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "CategoryName",
                table: "EventCategory",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "EventCategory",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventCategory",
                table: "EventCategory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_eventCategoryCategoryId",
                table: "Events",
                column: "eventCategoryCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_EventCategory_eventCategoryCategoryId",
                table: "Events",
                column: "eventCategoryCategoryId",
                principalTable: "EventCategory",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
