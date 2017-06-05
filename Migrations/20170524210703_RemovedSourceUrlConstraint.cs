using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace PodNoms.Api.Migrations
{
    public partial class RemovedSourceUrlConstraint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PodcastEntries_PodcastId_SourceUrl",
                table: "PodcastEntries");

            migrationBuilder.AlterColumn<string>(
                name: "SourceUrl",
                table: "PodcastEntries",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PodcastEntries_PodcastId",
                table: "PodcastEntries",
                column: "PodcastId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PodcastEntries_PodcastId",
                table: "PodcastEntries");

            migrationBuilder.AlterColumn<string>(
                name: "SourceUrl",
                table: "PodcastEntries",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PodcastEntries_PodcastId_SourceUrl",
                table: "PodcastEntries",
                columns: new[] { "PodcastId", "SourceUrl" },
                unique: true,
                filter: "[SourceUrl] IS NOT NULL");
        }
    }
}
