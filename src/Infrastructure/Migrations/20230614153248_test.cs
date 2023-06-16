using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ScheduleItem",
                table: "ScheduleItem");

            migrationBuilder.AddColumn<int>(
                name: "ID",
                table: "ScheduleItem",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ScheduleItem",
                table: "ScheduleItem",
                column: "ID");

            migrationBuilder.CreateTable(
                name: "ResourceSpecialCharacteristics",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TotalCapacity = table.Column<int>(type: "int", nullable: false),
                    AvailableCapacity = table.Column<int>(type: "int", nullable: false),
                    ScheduleID = table.Column<int>(type: "int", nullable: true),
                    ResourceID = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceSpecialCharacteristics", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ResourceSpecialCharacteristics_Resource_ResourceID",
                        column: x => x.ResourceID,
                        principalTable: "Resource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResourceSpecialCharacteristics_ScheduleItem_ScheduleID",
                        column: x => x.ScheduleID,
                        principalTable: "ScheduleItem",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleItem_ScheduleId",
                table: "ScheduleItem",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceSpecialCharacteristics_ResourceID",
                table: "ResourceSpecialCharacteristics",
                column: "ResourceID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResourceSpecialCharacteristics_ScheduleID",
                table: "ResourceSpecialCharacteristics",
                column: "ScheduleID",
                unique: true,
                filter: "[ScheduleID] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResourceSpecialCharacteristics");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ScheduleItem",
                table: "ScheduleItem");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleItem_ScheduleId",
                table: "ScheduleItem");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "ScheduleItem");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ScheduleItem",
                table: "ScheduleItem",
                columns: new[] { "ScheduleId", "Day", "StartTime", "EndTime" });
        }
    }
}
