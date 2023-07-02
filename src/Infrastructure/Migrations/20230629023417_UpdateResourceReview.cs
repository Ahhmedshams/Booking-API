using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateResourceReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BookingId",
                table: "ResourceReview",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ResourceReview_BookingId",
                table: "ResourceReview",
                column: "BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceReview_ClientBookings_BookingId",
                table: "ResourceReview",
                column: "BookingId",
                principalTable: "ClientBookings",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResourceReview_ClientBookings_BookingId",
                table: "ResourceReview");

            migrationBuilder.DropIndex(
                name: "IX_ResourceReview_BookingId",
                table: "ResourceReview");

            migrationBuilder.DropColumn(
                name: "BookingId",
                table: "ResourceReview");
        }
    }
}
