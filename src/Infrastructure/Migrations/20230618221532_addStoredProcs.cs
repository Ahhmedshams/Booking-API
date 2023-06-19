using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addStoredProcs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql(@"alter proc FillBookingItemTableWithScheduleInvisible @BookingID int ,@output int output 
with encryption 
as
CREATE TABLE #prices (
    id INT,
	price decimal 
);
CREATE TABLE #TempResourceTypeIDWithScheduleInvisible  (
    ResourceTypeId INT,
	NoOfResourcesRequired int
) 
--declare @output int ;
BEGIN TRY 
BEGIN TRANSACTION; --2

		insert into 
		#TempResourceTypeIDWithScheduleInvisible (ResourceTypeId ,NoOfResourcesRequired)
		select SM.[ResourceTypeId],
		SM.[NoOfResources]
		from [dbo].[ClientBookings] CB,
		[dbo].[ServiceMetadata] as SM,
		[dbo].[ResourceTypes] RS
		where CB.[Id]=@BookingID 
		and SM.[ServiceId]=CB.[ServiceId]
		and RS.[Id]=SM.[ResourceTypeId] 
		and [Shown]=0 
		and [HasSchedual]=1

--select * from #TempResourceTypeIDWithScheduleInvisible

	insert into #prices
	SELECT TOP 1 R.[Id] ,[Price]
		from [dbo].[Resource] R,
		#TempResourceTypeIDWithScheduleInvisible TRS,
		[dbo].[ClientBookings] CB,
		[dbo].[ScheduleItem] SI,
        [dbo].[ServiceMetadata] SM,
		[dbo].[ResourceSpecialCharacteristics] AS RSC 
		WHERE 
		 CB.[Id]=@BookingID 
        AND SI.[ID]=RSC.[ScheduleID]
		AND SM.[ResourceTypeId]=TRS.ResourceTypeId
		AND SM.[ServiceId]=CB.[ServiceId]
		AND TRS.ResourceTypeId=R.[ResourceTypeId]
		AND CB.[Date]=SI.[Day]
		AND CB.[StartTime]=SI.[StartTime]
		AND CB.[EndTime]=SI.[EndTime]
		AND SI.[Available]=1
		AND RSC.AvailableCapacity >= SM.[NoOfResources] 

--select * from  #prices

		UPDATE [dbo].[ResourceSpecialCharacteristics]
		SET [AvailableCapacity] = [AvailableCapacity]-TRS.NoOfResourcesRequired
		FROM #TempResourceTypeIDWithScheduleInvisible TRS,
		[dbo].[ResourceSpecialCharacteristics] RSC,
		#prices
		WHERE
		RSC.[ResourceID]=#prices.ID

		UPDATE [dbo].[ScheduleItem]
		SET [Available]=0
		FROM [dbo].[ScheduleItem] SI,
		[dbo].[ResourceSpecialCharacteristics] RSC
		WHERE RSC.[AvailableCapacity]=0
		AND RSC.[ScheduleID]=SI.[ID]

		INSERT INTO [dbo].[BookingItems] 
		([BookingId], [ResourceId],[Price])
		SELECT @BookingID, TP.[Id], TP.[Price]*TRS.NoOfResourcesRequired
		FROM #prices AS TP,#TempResourceTypeIDWithScheduleInvisible TRS

		set @output = 1;
		
COMMIT 
return @output;
END TRY
BEGIN CATCH
		ROLLBACK
		 set @output = 0; -- Set the output variable value
		 return @output;
END CATCH");
			migrationBuilder.Sql(@"alter proc FillBookingItemTableNoScheduleInvisible @BookingID int, @output int output
with encryption 
as
CREATE TABLE #prices2 (
    id INT,
	price int
);
--declare @output int ;
CREATE TABLE #TempResourceTypeIDWithNoScheduleInvisible  (
    ResourceTypeId INT,
	NoOfResourcesRequired int
) 
BEGIN TRY 
BEGIN TRANSACTION;
		insert into 
		#TempResourceTypeIDWithNoScheduleInvisible (ResourceTypeId ,NoOfResourcesRequired)
		select SM.[ResourceTypeId],
		SM.[NoOfResources]
		from [dbo].[ClientBookings] CB,
		[dbo].[ServiceMetadata] as SM,
		[dbo].[ResourceTypes] RS
		where CB.[Id]=@BookingID 
		and SM.[ServiceId]=CB.[ServiceId]
		and RS.[Id]=SM.[ResourceTypeId] 
		and [Shown]=0 
		and [HasSchedual]=0
	
--SELECT * FROM #TempResourceTypeIDWithNoScheduleInvisible
	insert into #prices2
	SELECT TOP 1  R.[Id] ,[Price]
		from [dbo].[Resource] R,
		#TempResourceTypeIDWithNoScheduleInvisible  TRS,
		[dbo].[ResourceSpecialCharacteristics] RSC
		WHERE 
		TRS.ResourceTypeId= R.[ResourceTypeId]
		AND R.[Id]=RSC.[ResourceID]
		AND RSC.[AvailableCapacity]>=TRS.NoOfResourcesRequired 

--SELECT *  FROM #prices2

		UPDATE [dbo].[ResourceSpecialCharacteristics]
		SET AvailableCapacity =AvailableCapacity-TRS.NoOfResourcesRequired
		FROM  #prices2,#TempResourceTypeIDWithNoScheduleInvisible TRS,[dbo].[ResourceSpecialCharacteristics] RSC
		WHERE #prices2.id =RSC.[ResourceID]

		INSERT INTO [dbo].[BookingItems] ([BookingId], [ResourceId], [Price])
		SELECT @BookingID, TP.[Id],(TP.[Price]*#TempResourceTypeIDWithNoScheduleInvisible.NoOfResourcesRequired)
		FROM #prices2 AS TP,#TempResourceTypeIDWithNoScheduleInvisible ----------------------NOTE
		set @output = 1;
		
COMMIT 
return @output;
END TRY
BEGIN CATCH
		ROLLBACK
		 set @output = 0; -- Set the output variable value
		 return @output;
END CATCH");
			migrationBuilder.Sql(@"alter proc FillBookingItemTableWithScheduleShown @BookingID int , @ResourceID int ,@output int output
with encryption 
as
CREATE TABLE #TempResourceTypeID (
    ResourceTypeId INT,
	NoOfResourcesRequired int
);
CREATE TABLE #p (
    id INT,
	price DECIMAL
);	
--declare @output int ;

	declare @price DECIMAL ,@date date, @startTime time, @endTime time, @serviceID int 
			IF EXISTS 
			(
				select R.id 
				from Resource R, 
				[dbo].[ScheduleItem] SI,
		        [dbo].[Schedule] S
				where R.id =@ResourceID
				--AND SI.[ScheduleId]=S.[ScheduleID]
				--AND S.[ResourceId]=@ResourceID
				--AND SI.[Available]=1
				--AND SI.[Day]=@date
				--AND SI.[StartTime]=@startTime
				--AND SI.[EndTime]=@endTime
			)
			

				select @date = [Date],
					   @serviceID=[ServiceId],
					   @startTime=[StartTime],
					   @endTime=[EndTime]
				from [dbo].[ClientBookings]
				where [Id]=@BookingID
		
				insert into #p
				select [Id],[Price]
				from [dbo].[Resource] R,[dbo].[ServiceMetadata] SMD
				where id=@ResourceID
				AND SMD.[ServiceId]=@serviceID
				AND R.[ResourceTypeId]=SMD.[ResourceTypeId]

begin try 	 
BEGIN TRANSACTION;
DECLARE @RowCount INT;

		
		UPDATE [dbo].[ScheduleItem]
		SET [Available]=0
		FROM [dbo].[ScheduleItem] SI,
		[dbo].[Schedule] S
		WHERE
		S.[ScheduleID]=SI.[ScheduleId]
		AND S.[ResourceId]=@ResourceID
		AND SI.[StartTime]=@startTime
		AND SI.[EndTime]=@endTime
		AND SI.[Day]=@date
		AND SI.[Available]=1

		SET @RowCount = @@ROWCOUNT;

		IF @RowCount > 0
		begin 
			INSERT INTO [dbo].[BookingItems] ([BookingId], [ResourceId], [Price])
			SELECT @BookingID, TP.[Id], TP.[Price]
			FROM #p AS TP;
			set @output = 1;
		end
		else 
		begin 
			set @output = 0;
		end

COMMIT 
return @output;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
    ROLLBACK TRANSACTION
	set @output = 0; -- Set the output variable value
	return @output;
END CATCH");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql("DROP FillBookingItemTableWithScheduleInvisible");
			migrationBuilder.Sql("DROP FillBookingItemTableNoScheduleInvisible");
			migrationBuilder.Sql("DROP FillBookingItemTableWithScheduleShown");
        }
    }
}
