using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsWebSiteScraper.Data.Migrations
{
    public partial class CreatingNewsViewsTableForTrackingUniqueViewsToNewsAndAddingUniqueNewsPropertyToTheEntityNews : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UniqueViews",
                table: "News",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "NewsViews",
                columns: table => new
                {
                    NewsId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsViews", x => new { x.NewsId, x.UserId });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NewsViews");

            migrationBuilder.DropColumn(
                name: "UniqueViews",
                table: "News");
        }
    }
}
