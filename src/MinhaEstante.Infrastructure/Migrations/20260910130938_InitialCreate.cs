using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhaEstante.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Author = table.Column<string>(type: "TEXT", nullable: true),
                    Format = table.Column<string>(type: "TEXT", nullable: false),
                    FilePath = table.Column<string>(type: "TEXT", nullable: false),
                    CoverImagePath = table.Column<string>(type: "TEXT", nullable: true),
                    AddedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    TotalPages = table.Column<int>(type: "INTEGER", nullable: false),
                    Genre = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReadingProgress",
                columns: table => new
                {
                    BookId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CurrentPage = table.Column<int>(type: "INTEGER", nullable: false),
                    LastReadAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReadingProgress", x => x.BookId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "ReadingProgress");
        }
    }
}
