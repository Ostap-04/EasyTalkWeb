using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyTalkWeb.Migrations
{
    /// <inheritdoc />
    public partial class addTechnologiesRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobPostTechnology",
                columns: table => new
                {
                    JobpostsId = table.Column<Guid>(type: "uuid", nullable: false),
                    TechnologiesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobPostTechnology", x => new { x.JobpostsId, x.TechnologiesId });
                    table.ForeignKey(
                        name: "FK_JobPostTechnology_JobPosts_JobpostsId",
                        column: x => x.JobpostsId,
                        principalTable: "JobPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobPostTechnology_Technologies_TechnologiesId",
                        column: x => x.TechnologiesId,
                        principalTable: "Technologies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProposalTechnology",
                columns: table => new
                {
                    ProposalsId = table.Column<Guid>(type: "uuid", nullable: false),
                    TechnologiesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProposalTechnology", x => new { x.ProposalsId, x.TechnologiesId });
                    table.ForeignKey(
                        name: "FK_ProposalTechnology_Proposals_ProposalsId",
                        column: x => x.ProposalsId,
                        principalTable: "Proposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProposalTechnology_Technologies_TechnologiesId",
                        column: x => x.TechnologiesId,
                        principalTable: "Technologies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobPostTechnology_TechnologiesId",
                table: "JobPostTechnology",
                column: "TechnologiesId");

            migrationBuilder.CreateIndex(
                name: "IX_ProposalTechnology_TechnologiesId",
                table: "ProposalTechnology",
                column: "TechnologiesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobPostTechnology");

            migrationBuilder.DropTable(
                name: "ProposalTechnology");
        }
    }
}
