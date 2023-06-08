using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComUnity.Application.Database.Migrations
{
    /// <inheritdoc />
    public partial class imageCat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "EventCategory");

            migrationBuilder.AddColumn<Guid>(
                name: "ImageId",
                table: "EventCategory",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "EventCategory");

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "EventCategory",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
