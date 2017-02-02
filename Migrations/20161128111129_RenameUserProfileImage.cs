using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PodNoms.Api.Migrations
{
    public partial class RenameUserProfileImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.RenameColumn("AvatarUrl", "Users", "ProfileImage");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn("ProfileImage", "Users", "AvatarUrl");
        }
    }
}
