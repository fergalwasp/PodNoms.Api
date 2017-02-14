using Microsoft.EntityFrameworkCore.Migrations;

namespace PodNoms.Api.Migrations
{
    public partial class AddedApiKeyToUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApiKey",
                table: "Users",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApiKey",
                table: "Users");
        }
    }
}
