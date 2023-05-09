using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjGrupowy.Server.Migrations
{
    /// <inheritdoc />
    public partial class addEventCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "eventCategoryCategoryId",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EventCategory",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventCategory", x => x.CategoryId);
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_EventCategory_eventCategoryCategoryId",
                table: "Events");

            migrationBuilder.DropTable(
                name: "EventCategory");

            migrationBuilder.DropIndex(
                name: "IX_Events_eventCategoryCategoryId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "eventCategoryCategoryId",
                table: "Events");
        }
    }
}
