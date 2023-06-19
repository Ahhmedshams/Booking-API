using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addStoredCalculateTotalCost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"alter proc CalculateTotalCost @bookingID int
with encryption 
as
BEGIN TRY
BEGIN TRANSACTION;
DECLARE @TOTAL INT 

	select  @TOTAL=sum ([Price])
	from [dbo].[BookingItems] BI
	where BI.[BookingId]=@bookingID

	UPDATE [dbo].[ClientBookings]
	SET [TotalCost] = @TOTAL
	WHERE [Id]=@bookingID
COMMIT 
END TRY
BEGIN CATCH
    SELECT 0
    ROLLBACK TRANSACTION
END CATCH");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP CalculateTotalCost");
        }
    }
}