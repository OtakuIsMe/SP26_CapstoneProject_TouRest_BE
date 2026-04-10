using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TouRest.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _20260410 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "providers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "CreateByUserId",
                table: "providers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "EndTime",
                table: "providers",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "providers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "providers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "StartTime",
                table: "providers",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<int>(
                name: "PicNumber",
                table: "images",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "TypeId",
                table: "images",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "agencies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "CreateByUserId",
                table: "agencies",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "EndTime",
                table: "agencies",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "agencies",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "agencies",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "StartTime",
                table: "agencies",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.CreateIndex(
                name: "IX_providers_CreateByUserId",
                table: "providers",
                column: "CreateByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_agencies_CreateByUserId",
                table: "agencies",
                column: "CreateByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_agencies_users_CreateByUserId",
                table: "agencies",
                column: "CreateByUserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_providers_users_CreateByUserId",
                table: "providers",
                column: "CreateByUserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_agencies_users_CreateByUserId",
                table: "agencies");

            migrationBuilder.DropForeignKey(
                name: "FK_providers_users_CreateByUserId",
                table: "providers");

            migrationBuilder.DropIndex(
                name: "IX_providers_CreateByUserId",
                table: "providers");

            migrationBuilder.DropIndex(
                name: "IX_agencies_CreateByUserId",
                table: "agencies");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "providers");

            migrationBuilder.DropColumn(
                name: "CreateByUserId",
                table: "providers");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "providers");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "providers");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "providers");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "providers");

            migrationBuilder.DropColumn(
                name: "PicNumber",
                table: "images");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "images");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "agencies");

            migrationBuilder.DropColumn(
                name: "CreateByUserId",
                table: "agencies");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "agencies");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "agencies");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "agencies");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "agencies");
        }
    }
}
