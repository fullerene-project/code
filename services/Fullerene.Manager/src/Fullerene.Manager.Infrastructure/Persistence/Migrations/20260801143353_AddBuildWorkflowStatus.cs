using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fullerene.Manager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBuildWorkflowStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "BuildWorkflows",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "BuildWorkflows");
        }
    }
}
