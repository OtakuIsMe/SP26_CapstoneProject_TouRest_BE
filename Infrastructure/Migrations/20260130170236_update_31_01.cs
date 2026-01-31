using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TouRest.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class update_31_01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_itinerary_stops_ItineraryId",
                table: "itinerary_stops");

            migrationBuilder.DropIndex(
                name: "IX_itinerary_activities_ItineraryStopId",
                table: "itinerary_activities");

            migrationBuilder.DropIndex(
                name: "IX_booking_itineraries_BookingId",
                table: "booking_itineraries");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "users",
                newName: "PasswordHash");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "roles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedByIp = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    RevokedByIp = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_tokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_refresh_tokens_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "Id", "Code", "CreatedAt", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "CUSTOMER", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Khách hàng", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "ADMIN", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Quản trị viên", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "PROVIDER", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Nhà cung cấp dịch vụ", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "AGENCY", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Đại lý du lịch", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_wishlists_ItemId_UserId",
                table: "wishlists",
                columns: new[] { "ItemId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_Email",
                table: "users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_roles_Code",
                table: "roles",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_providers_ContactEmail",
                table: "providers",
                column: "ContactEmail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_packages_Code",
                table: "packages",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_package_services_PackageId_SortOrder",
                table: "package_services",
                columns: new[] { "PackageId", "SortOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_itinerary_stops_ItineraryId_StopOrder",
                table: "itinerary_stops",
                columns: new[] { "ItineraryId", "StopOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_itinerary_activities_ItineraryStopId_ActivityOrder",
                table: "itinerary_activities",
                columns: new[] { "ItineraryStopId", "ActivityOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_bookings_Code",
                table: "bookings",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_booking_itineraries_BookingId_ItineraryId",
                table: "booking_itineraries",
                columns: new[] { "BookingId", "ItineraryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_UserId",
                table: "refresh_tokens",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropIndex(
                name: "IX_wishlists_ItemId_UserId",
                table: "wishlists");

            migrationBuilder.DropIndex(
                name: "IX_users_Email",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_roles_Code",
                table: "roles");

            migrationBuilder.DropIndex(
                name: "IX_providers_ContactEmail",
                table: "providers");

            migrationBuilder.DropIndex(
                name: "IX_packages_Code",
                table: "packages");

            migrationBuilder.DropIndex(
                name: "IX_package_services_PackageId_SortOrder",
                table: "package_services");

            migrationBuilder.DropIndex(
                name: "IX_itinerary_stops_ItineraryId_StopOrder",
                table: "itinerary_stops");

            migrationBuilder.DropIndex(
                name: "IX_itinerary_activities_ItineraryStopId_ActivityOrder",
                table: "itinerary_activities");

            migrationBuilder.DropIndex(
                name: "IX_bookings_Code",
                table: "bookings");

            migrationBuilder.DropIndex(
                name: "IX_booking_itineraries_BookingId_ItineraryId",
                table: "booking_itineraries");

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.DropColumn(
                name: "Code",
                table: "roles");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "users",
                newName: "Password");

            migrationBuilder.CreateIndex(
                name: "IX_itinerary_stops_ItineraryId",
                table: "itinerary_stops",
                column: "ItineraryId");

            migrationBuilder.CreateIndex(
                name: "IX_itinerary_activities_ItineraryStopId",
                table: "itinerary_activities",
                column: "ItineraryStopId");

            migrationBuilder.CreateIndex(
                name: "IX_booking_itineraries_BookingId",
                table: "booking_itineraries",
                column: "BookingId");
        }
    }
}
