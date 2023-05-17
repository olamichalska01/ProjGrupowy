using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComUnity.Application.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddEventsandEventCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "UserProfile",
                newName: "Username");

            migrationBuilder.CreateTable(
                name: "EventCategory",
                columns: table => new
                {
                    CategoryName = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventCategory", x => x.CategoryName);
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
                    EventCategoryCategoryName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EventCategoryCategoryName1 = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Event_EventCategory_EventCategoryCategoryName",
                        column: x => x.EventCategoryCategoryName,
                        principalTable: "EventCategory",
                        principalColumn: "CategoryName",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Event_EventCategory_EventCategoryCategoryName1",
                        column: x => x.EventCategoryCategoryName1,
                        principalTable: "EventCategory",
                        principalColumn: "CategoryName");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Event_EventCategoryCategoryName",
                table: "Event",
                column: "EventCategoryCategoryName");

            migrationBuilder.CreateIndex(
                name: "IX_Event_EventCategoryCategoryName1",
                table: "Event",
                column: "EventCategoryCategoryName1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Event");

            migrationBuilder.DropTable(
                name: "EventCategory");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "UserProfile",
                newName: "UserName");
        }
    }
}
