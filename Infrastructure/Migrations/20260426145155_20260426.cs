using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TouRest.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _20260426 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AgencyReply",
                table: "feedbacks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RepliedAt",
                table: "feedbacks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RepliedByUserId",
                table: "feedbacks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_feedbacks_RepliedByUserId",
                table: "feedbacks",
                column: "RepliedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_feedbacks_users_RepliedByUserId",
                table: "feedbacks",
                column: "RepliedByUserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_feedbacks_users_RepliedByUserId",
                table: "feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_feedbacks_RepliedByUserId",
                table: "feedbacks");

            migrationBuilder.DropColumn(
                name: "AgencyReply",
                table: "feedbacks");

            migrationBuilder.DropColumn(
                name: "RepliedAt",
                table: "feedbacks");

            migrationBuilder.DropColumn(
                name: "RepliedByUserId",
                table: "feedbacks");
        }
    }
}
