using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhaEstante.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGeneralSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "Settings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ProgressSaveIntervalSeconds",
                table: "Settings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Language",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "ProgressSaveIntervalSeconds",
                table: "Settings");
        }
    }
}
