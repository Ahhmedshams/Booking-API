using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Tickets2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_AspNetUsers_ApplicationUserId",
                table: "Ticket");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketReviewed_AspNetUsers_ApplicationUserId",
                table: "TicketReviewed");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketReviewed_Ticket_TicketId",
                table: "TicketReviewed");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TicketReviewed",
                table: "TicketReviewed");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ticket",
                table: "Ticket");

            migrationBuilder.RenameTable(
                name: "TicketReviewed",
                newName: "TicketRevieweds");

            migrationBuilder.RenameTable(
                name: "Ticket",
                newName: "Tickets");

            migrationBuilder.RenameIndex(
                name: "IX_TicketReviewed_TicketId",
                table: "TicketRevieweds",
                newName: "IX_TicketRevieweds_TicketId");

            migrationBuilder.RenameIndex(
                name: "IX_TicketReviewed_ApplicationUserId",
                table: "TicketRevieweds",
                newName: "IX_TicketRevieweds_ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Ticket_ApplicationUserId",
                table: "Tickets",
                newName: "IX_Tickets_ApplicationUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TicketRevieweds",
                table: "TicketRevieweds",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tickets",
                table: "Tickets",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketRevieweds_AspNetUsers_ApplicationUserId",
                table: "TicketRevieweds",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketRevieweds_Tickets_TicketId",
                table: "TicketRevieweds",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_AspNetUsers_ApplicationUserId",
                table: "Tickets",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketRevieweds_AspNetUsers_ApplicationUserId",
                table: "TicketRevieweds");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketRevieweds_Tickets_TicketId",
                table: "TicketRevieweds");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_AspNetUsers_ApplicationUserId",
                table: "Tickets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tickets",
                table: "Tickets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TicketRevieweds",
                table: "TicketRevieweds");

            migrationBuilder.RenameTable(
                name: "Tickets",
                newName: "Ticket");

            migrationBuilder.RenameTable(
                name: "TicketRevieweds",
                newName: "TicketReviewed");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_ApplicationUserId",
                table: "Ticket",
                newName: "IX_Ticket_ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_TicketRevieweds_TicketId",
                table: "TicketReviewed",
                newName: "IX_TicketReviewed_TicketId");

            migrationBuilder.RenameIndex(
                name: "IX_TicketRevieweds_ApplicationUserId",
                table: "TicketReviewed",
                newName: "IX_TicketReviewed_ApplicationUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ticket",
                table: "Ticket",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TicketReviewed",
                table: "TicketReviewed",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_AspNetUsers_ApplicationUserId",
                table: "Ticket",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketReviewed_AspNetUsers_ApplicationUserId",
                table: "TicketReviewed",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketReviewed_Ticket_TicketId",
                table: "TicketReviewed",
                column: "TicketId",
                principalTable: "Ticket",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
