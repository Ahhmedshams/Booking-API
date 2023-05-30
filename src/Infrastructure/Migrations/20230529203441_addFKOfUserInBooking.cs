using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addFKOfUserInBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingItems_ClientBookings_ClientBookingId",
                table: "BookingItems");

            migrationBuilder.DropIndex(
                name: "IX_BookingItems_ClientBookingId",
                table: "BookingItems");

            migrationBuilder.DropColumn(
                name: "ClientBookingId",
                table: "BookingItems");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ClientBookings",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_ClientBookings_UserId",
                table: "ClientBookings",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientBookings_AspNetUsers_UserId",
                table: "ClientBookings",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientBookings_AspNetUsers_UserId",
                table: "ClientBookings");

            migrationBuilder.DropIndex(
                name: "IX_ClientBookings_UserId",
                table: "ClientBookings");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "ClientBookings",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "ClientBookingId",
                table: "BookingItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookingItems_ClientBookingId",
                table: "BookingItems",
                column: "ClientBookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingItems_ClientBookings_ClientBookingId",
                table: "BookingItems",
                column: "ClientBookingId",
                principalTable: "ClientBookings",
                principalColumn: "Id");
        }
    }
}
