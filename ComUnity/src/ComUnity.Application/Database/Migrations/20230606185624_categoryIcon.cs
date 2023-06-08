using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComUnity.Application.Database.Migrations
{
    /// <inheritdoc />
    public partial class categoryIcon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "EventCategory",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "EventCategory");
        }
    }
}
