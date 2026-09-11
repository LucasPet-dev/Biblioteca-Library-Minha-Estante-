using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhaEstante.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveReaderAppearanceLibrarySettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccentColor",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "ReaderBackground",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "ReadingDirection",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "ShowAuthorOnCard",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "ShowGenreOnCard",
                table: "Settings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccentColor",
                table: "Settings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReaderBackground",
                table: "Settings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReadingDirection",
                table: "Settings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

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
        }
    }
}
