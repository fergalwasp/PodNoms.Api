using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PodNoms.Api.Migrations
{
    public partial class AddedUserFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AvatarUrl = table.Column<string>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    EmailAddress = table.Column<string>(nullable: true),
                    FullName = table.Column<string>(nullable: true),
                    UpdateDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Podcasts",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Podcasts_UserId",
                table: "Podcasts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Podcasts_Users_UserId",
                table: "Podcasts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Podcasts_Users_UserId",
                table: "Podcasts");

            migrationBuilder.DropIndex(
                name: "IX_Podcasts_UserId",
                table: "Podcasts");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Podcasts",
                nullable: true);
        }
    }
}
