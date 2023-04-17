using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsWebSiteScraper.Data.Migrations
{
    public partial class ChengedNewsEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "content",
                table: "News",
                newName: "Content");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Content",
                table: "News",
                newName: "content");
        }
    }
}
