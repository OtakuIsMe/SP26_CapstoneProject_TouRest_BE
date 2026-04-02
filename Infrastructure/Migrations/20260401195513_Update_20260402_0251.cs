using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TouRest.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Update_20260402_0251 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_booking_itineraries_itineraries_ItineraryId",
                table: "booking_itineraries");

            migrationBuilder.DropForeignKey(
                name: "FK_feedbacks_bookings_BookingId",
                table: "feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_itineraries_users_AgencyId",
                table: "itineraries");

            migrationBuilder.RenameColumn(
                name: "BookingId",
                table: "feedbacks",
                newName: "BookingItineraryId");

            migrationBuilder.RenameIndex(
                name: "IX_feedbacks_BookingId",
                table: "feedbacks",
                newName: "IX_feedbacks_BookingItineraryId");

            migrationBuilder.RenameColumn(
                name: "ItineraryId",
                table: "booking_itineraries",
                newName: "ItineraryScheduleId");

            migrationBuilder.RenameIndex(
                name: "IX_booking_itineraries_ItineraryId",
                table: "booking_itineraries",
                newName: "IX_booking_itineraries_ItineraryScheduleId");

            migrationBuilder.RenameIndex(
                name: "IX_booking_itineraries_BookingId_ItineraryId",
                table: "booking_itineraries",
                newName: "IX_booking_itineraries_BookingId_ItineraryScheduleId");

            migrationBuilder.CreateTable(
                name: "agencies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ContactEmail = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ContactPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_agencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "itinerary_schedule",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItineraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_itinerary_schedule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_itinerary_schedule_itineraries_ItineraryId",
                        column: x => x.ItineraryId,
                        principalTable: "itineraries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "agency_users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AgencyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_agency_users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_agency_users_agencies_AgencyId",
                        column: x => x.AgencyId,
                        principalTable: "agencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_agency_users_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "itinerary_tracking",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItineraryScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrackingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_itinerary_tracking", x => x.Id);
                    table.ForeignKey(
                        name: "FK_itinerary_tracking_itinerary_schedule_ItineraryScheduleId",
                        column: x => x.ItineraryScheduleId,
                        principalTable: "itinerary_schedule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_agency_users_AgencyId",
                table: "agency_users",
                column: "AgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_agency_users_UserId",
                table: "agency_users",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_itinerary_schedule_ItineraryId",
                table: "itinerary_schedule",
                column: "ItineraryId");

            migrationBuilder.CreateIndex(
                name: "IX_itinerary_tracking_ItineraryScheduleId",
                table: "itinerary_tracking",
                column: "ItineraryScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_booking_itineraries_itinerary_schedule_ItineraryScheduleId",
                table: "booking_itineraries",
                column: "ItineraryScheduleId",
                principalTable: "itinerary_schedule",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_feedbacks_booking_itineraries_BookingItineraryId",
                table: "feedbacks",
                column: "BookingItineraryId",
                principalTable: "booking_itineraries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_itineraries_agencies_AgencyId",
                table: "itineraries",
                column: "AgencyId",
                principalTable: "agencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_booking_itineraries_itinerary_schedule_ItineraryScheduleId",
                table: "booking_itineraries");

            migrationBuilder.DropForeignKey(
                name: "FK_feedbacks_booking_itineraries_BookingItineraryId",
                table: "feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_itineraries_agencies_AgencyId",
                table: "itineraries");

            migrationBuilder.DropTable(
                name: "agency_users");

            migrationBuilder.DropTable(
                name: "itinerary_tracking");

            migrationBuilder.DropTable(
                name: "agencies");

            migrationBuilder.DropTable(
                name: "itinerary_schedule");

            migrationBuilder.RenameColumn(
                name: "BookingItineraryId",
                table: "feedbacks",
                newName: "BookingId");

            migrationBuilder.RenameIndex(
                name: "IX_feedbacks_BookingItineraryId",
                table: "feedbacks",
                newName: "IX_feedbacks_BookingId");

            migrationBuilder.RenameColumn(
                name: "ItineraryScheduleId",
                table: "booking_itineraries",
                newName: "ItineraryId");

            migrationBuilder.RenameIndex(
                name: "IX_booking_itineraries_ItineraryScheduleId",
                table: "booking_itineraries",
                newName: "IX_booking_itineraries_ItineraryId");

            migrationBuilder.RenameIndex(
                name: "IX_booking_itineraries_BookingId_ItineraryScheduleId",
                table: "booking_itineraries",
                newName: "IX_booking_itineraries_BookingId_ItineraryId");

            migrationBuilder.AddForeignKey(
                name: "FK_booking_itineraries_itineraries_ItineraryId",
                table: "booking_itineraries",
                column: "ItineraryId",
                principalTable: "itineraries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_feedbacks_bookings_BookingId",
                table: "feedbacks",
                column: "BookingId",
                principalTable: "bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_itineraries_users_AgencyId",
                table: "itineraries",
                column: "AgencyId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
