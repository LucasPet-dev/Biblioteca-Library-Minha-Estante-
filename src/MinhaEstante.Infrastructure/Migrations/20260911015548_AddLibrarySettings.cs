using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhaEstante.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLibrarySettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ShowAuthorOnCard",
                table: "Settings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowGenreOnCard",
                table: "Settings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SortOrder",
                table: "Settings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShowAuthorOnCard",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "ShowGenreOnCard",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "Settings");
        }
    }
}
