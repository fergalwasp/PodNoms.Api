using Microsoft.EntityFrameworkCore.Migrations;

namespace PodNoms.Api.Migrations
{
    public partial class AddedSlugFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "PodcastEntries",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Podcasts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Slug",
                table: "PodcastEntries");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Podcasts");
        }
    }
}
