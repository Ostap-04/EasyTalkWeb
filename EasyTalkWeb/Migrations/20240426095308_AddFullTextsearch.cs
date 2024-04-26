using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyTalkWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddFullTextsearch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Freelancers_Specialization",
                table: "Freelancers",
                column: "Specialization")
                .Annotation("Npgsql:IndexMethod", "GIN")
                .Annotation("Npgsql:TsVectorConfig", "english");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Freelancers_Specialization",
                table: "Freelancers");
        }
    }
}
