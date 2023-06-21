using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addResourceRegion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Resource_Regions_RegionId",
                table: "Resource");

            migrationBuilder.DropIndex(
                name: "IX_Resource_RegionId",
                table: "Resource");

            migrationBuilder.RenameColumn(
                name: "RegionId",
                table: "Resource",
                newName: "ResourceRegionId");

            migrationBuilder.AddColumn<int>(
                name: "ResourceRegionId",
                table: "Regions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ResourceRegion",
                columns: table => new
                {
                    ResourceId = table.Column<int>(type: "int", nullable: false),
                    RegionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceRegion", x => new { x.ResourceId, x.RegionId });
                    table.ForeignKey(
                        name: "FK_ResourceRegion_Regions_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Regions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResourceRegion_Resource_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResourceRegion_RegionId",
                table: "ResourceRegion",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceRegion_ResourceId",
                table: "ResourceRegion",
                column: "ResourceId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResourceRegion");

            migrationBuilder.DropColumn(
                name: "ResourceRegionId",
                table: "Regions");

            migrationBuilder.RenameColumn(
                name: "ResourceRegionId",
                table: "Resource",
                newName: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_Resource_RegionId",
                table: "Resource",
                column: "RegionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Resource_Regions_RegionId",
                table: "Resource",
                column: "RegionId",
                principalTable: "Regions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
