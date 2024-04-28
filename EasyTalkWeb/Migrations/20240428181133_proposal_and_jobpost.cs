using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyTalkWeb.Migrations
{
    /// <inheritdoc />
    public partial class proposal_and_jobpost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "JobPostId",
                table: "Proposals",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Proposals_JobPostId",
                table: "Proposals",
                column: "JobPostId");

            migrationBuilder.AddForeignKey(
                name: "FK_Proposals_JobPosts_JobPostId",
                table: "Proposals",
                column: "JobPostId",
                principalTable: "JobPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proposals_JobPosts_JobPostId",
                table: "Proposals");

            migrationBuilder.DropIndex(
                name: "IX_Proposals_JobPostId",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "JobPostId",
                table: "Proposals");
        }
    }
}
