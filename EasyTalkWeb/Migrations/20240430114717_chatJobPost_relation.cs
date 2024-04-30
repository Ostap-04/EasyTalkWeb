using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyTalkWeb.Migrations
{
    /// <inheritdoc />
    public partial class chatJobPost_relation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ChatId",
                table: "JobPosts",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobPosts_ChatId",
                table: "JobPosts",
                column: "ChatId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_JobPosts_Chats_ChatId",
                table: "JobPosts",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobPosts_Chats_ChatId",
                table: "JobPosts");

            migrationBuilder.DropIndex(
                name: "IX_JobPosts_ChatId",
                table: "JobPosts");

            migrationBuilder.DropColumn(
                name: "ChatId",
                table: "JobPosts");
        }
    }
}
