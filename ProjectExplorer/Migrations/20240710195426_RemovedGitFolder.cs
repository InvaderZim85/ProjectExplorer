using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectExplorer.Migrations
{
    /// <inheritdoc />
    public partial class RemovedGitFolder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GitFolder",
                table: "Project");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GitFolder",
                table: "Project",
                type: "TEXT",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }
    }
}
