using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyTalkWeb.Migrations
{
    /// <inheritdoc />
    public partial class updateprojectconfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FreelancerProject");

            migrationBuilder.AddColumn<Guid>(
                name: "FreelancerId",
                table: "Projects",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Projects_FreelancerId",
                table: "Projects",
                column: "FreelancerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Freelancers_FreelancerId",
                table: "Projects",
                column: "FreelancerId",
                principalTable: "Freelancers",
                principalColumn: "FreelancerId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Freelancers_FreelancerId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_FreelancerId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "FreelancerId",
                table: "Projects");

            migrationBuilder.CreateTable(
                name: "FreelancerProject",
                columns: table => new
                {
                    FreelancersFreelancerId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FreelancerProject", x => new { x.FreelancersFreelancerId, x.ProjectsId });
                    table.ForeignKey(
                        name: "FK_FreelancerProject_Freelancers_FreelancersFreelancerId",
                        column: x => x.FreelancersFreelancerId,
                        principalTable: "Freelancers",
                        principalColumn: "FreelancerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FreelancerProject_Projects_ProjectsId",
                        column: x => x.ProjectsId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FreelancerProject_ProjectsId",
                table: "FreelancerProject",
                column: "ProjectsId");
        }
    }
}
