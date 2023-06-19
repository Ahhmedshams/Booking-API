using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addStoredConfirmedComplited : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"alter proc SetBookingStatusConfiremed @bookingID int
with encryption 
as
	IF EXISTS 
			(
				select id
				from [dbo].[ClientBookings]
				where [Id]= @bookingID 
			)
	begin 

		update [dbo].[ClientBookings]
		set [Status]= 'Confiremed '
		where [Id]=@bookingID
	end 
	else 
	return 0");
            migrationBuilder.Sql(@"alter PROCEDURE SetBookingStatuscompleted
    @bookingID INT
WITH ENCRYPTION
AS
BEGIN TRY
    UPDATE [dbo].[ClientBookings]
    SET [Status] = 'Completed'
    WHERE [Id] = @bookingID
        AND [Date] = CONVERT(DATE, GETDATE())
        AND CAST([StartTime] AS TIME) <= CAST(CURRENT_TIMESTAMP AS TIME)
END TRY
BEGIN CATCH 
	RETURN 0
END CATCH");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP SetBookingStatusConfiremed");
            migrationBuilder.Sql("DROP SetBookingStatuscompleted");
        }
    }
}
