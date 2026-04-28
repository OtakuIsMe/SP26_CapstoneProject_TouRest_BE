using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TouRest.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _20260422 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_booking_itineraries_bookings_BookingId1",
                table: "booking_itineraries");

            migrationBuilder.DropForeignKey(
                name: "FK_itineraries_agency_users_TourGuideId",
                table: "itineraries");

            migrationBuilder.DropForeignKey(
                name: "FK_itinerary_stops_Vehicle_VehicleId",
                table: "itinerary_stops");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicle_agencies_AgencyId",
                table: "Vehicle");

            migrationBuilder.DropIndex(
                name: "IX_booking_itineraries_BookingId1",
                table: "booking_itineraries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vehicle",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "BookingId1",
                table: "booking_itineraries");

            migrationBuilder.RenameTable(
                name: "Vehicle",
                newName: "Vehicles");

            migrationBuilder.RenameIndex(
                name: "IX_Vehicle_AgencyId",
                table: "Vehicles",
                newName: "IX_Vehicles_AgencyId");

            migrationBuilder.AddColumn<Guid>(
                name: "ItineraryId1",
                table: "itinerary_stops",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vehicles",
                table: "Vehicles",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_itinerary_stops_ItineraryId1",
                table: "itinerary_stops",
                column: "ItineraryId1");

            migrationBuilder.AddForeignKey(
                name: "FK_itineraries_agency_users_TourGuideId",
                table: "itineraries",
                column: "TourGuideId",
                principalTable: "agency_users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_itinerary_stops_Vehicles_VehicleId",
                table: "itinerary_stops",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_itinerary_stops_itineraries_ItineraryId1",
                table: "itinerary_stops",
                column: "ItineraryId1",
                principalTable: "itineraries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_agencies_AgencyId",
                table: "Vehicles",
                column: "AgencyId",
                principalTable: "agencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_itineraries_agency_users_TourGuideId",
                table: "itineraries");

            migrationBuilder.DropForeignKey(
                name: "FK_itinerary_stops_Vehicles_VehicleId",
                table: "itinerary_stops");

            migrationBuilder.DropForeignKey(
                name: "FK_itinerary_stops_itineraries_ItineraryId1",
                table: "itinerary_stops");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_agencies_AgencyId",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_itinerary_stops_ItineraryId1",
                table: "itinerary_stops");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vehicles",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "ItineraryId1",
                table: "itinerary_stops");

            migrationBuilder.RenameTable(
                name: "Vehicles",
                newName: "Vehicle");

            migrationBuilder.RenameIndex(
                name: "IX_Vehicles_AgencyId",
                table: "Vehicle",
                newName: "IX_Vehicle_AgencyId");

            migrationBuilder.AddColumn<Guid>(
                name: "BookingId1",
                table: "booking_itineraries",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vehicle",
                table: "Vehicle",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_booking_itineraries_BookingId1",
                table: "booking_itineraries",
                column: "BookingId1");

            migrationBuilder.AddForeignKey(
                name: "FK_booking_itineraries_bookings_BookingId1",
                table: "booking_itineraries",
                column: "BookingId1",
                principalTable: "bookings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_itineraries_agency_users_TourGuideId",
                table: "itineraries",
                column: "TourGuideId",
                principalTable: "agency_users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_itinerary_stops_Vehicle_VehicleId",
                table: "itinerary_stops",
                column: "VehicleId",
                principalTable: "Vehicle",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicle_agencies_AgencyId",
                table: "Vehicle",
                column: "AgencyId",
                principalTable: "agencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
