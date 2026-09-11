using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhaEstante.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAppSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DefaultZoom = table.Column<double>(type: "REAL", nullable: false),
                    FitMode = table.Column<string>(type: "TEXT", nullable: false),
                    ReadingDirection = table.Column<string>(type: "TEXT", nullable: false),
                    ReaderBackground = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Settings");
        }
    }
}
