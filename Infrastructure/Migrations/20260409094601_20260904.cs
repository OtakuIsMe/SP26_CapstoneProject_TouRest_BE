using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TouRest.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _20260904 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPrimaryContact",
                table: "providers");

            migrationBuilder.DropColumn(
                name: "IsPrimaryContact",
                table: "agency_users");

            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "agency_users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "agency_users");

            migrationBuilder.AddColumn<bool>(
                name: "IsPrimaryContact",
                table: "providers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPrimaryContact",
                table: "agency_users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
