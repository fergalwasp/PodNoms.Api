using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PodNoms.Api.Migrations
{
    public partial class FinerGrainedProcessingStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProcessingStatus",
                table: "PodcastEntries",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProcessingStatus",
                table: "PodcastEntries");
        }
    }
}
