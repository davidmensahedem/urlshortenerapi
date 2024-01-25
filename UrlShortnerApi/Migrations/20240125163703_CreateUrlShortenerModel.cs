using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlShortnerApi.Migrations
{
    /// <inheritdoc />
    public partial class CreateUrlShortenerModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UrlShortners",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    LongUrl = table.Column<string>(type: "text", nullable: true),
                    ShortUrl = table.Column<string>(type: "text", nullable: true),
                    Code = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UrlShortners", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UrlShortners_Code",
                table: "UrlShortners",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UrlShortners");
        }
    }
}
