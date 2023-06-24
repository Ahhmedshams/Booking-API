using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStoredProcs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Get Available Resources
            var proc1 = @"create PROC proc1
	@serviceID INT
WITH ENCRYPTION 
AS
BEGIN
declare @output int ;
	CREATE TABLE #TempResourceTypeIDs (
		ResourceTypeId INT,
		NoOfResourcesRequired INT
	);
	CREATE TABLE #TempVisibleResourceTypeIDsHasSchedule (
		ResourceTypeId INT
	);
	
	INSERT INTO #TempResourceTypeIDs (ResourceTypeId, NoOfResourcesRequired)
	SELECT [ResourceTypeId], NoOfResources
	FROM [dbo].[ServiceMetadata]
	WHERE [ServiceId] = @serviceID;

	IF EXISTS (
		SELECT 1
		FROM [dbo].[ResourceTypes], #TempResourceTypeIDs
		WHERE [Id] = ResourceTypeId AND [shown] = 1 AND [HasSchedual] = 1
	)
	BEGIN
		INSERT INTO #TempVisibleResourceTypeIDsHasSchedule (ResourceTypeId)
		SELECT [Id]
		FROM [dbo].[ResourceTypes], #TempResourceTypeIDs
		WHERE [Id] = ResourceTypeId
		AND [shown] = 1 AND [HasSchedual] = 1
		set @output = 1;
		--select * from #TempVisibleResourceTypeIDsHasSchedule
	END
	ELSE
	BEGIN
		set @output = 0; -- Set the output variable value
		return @output;
	END
	
	return @output; 
END;";

            migrationBuilder.Sql(proc1);


            var proc2 = @"create PROC proc2
@serviceID int		--shown 0 and schedule 1
with encryption 
as
BEGIN
declare @output INT
CREATE TABLE #TempResourceTypeIDs2 (
	ResourceTypeId INT,
	NoOfResourcesRequired int
);
CREATE TABLE #TempInVisibleResourceTypeIDsHasSchedule (
	ResourceTypeId INT,
);
	INSERT INTO #TempResourceTypeIDs2 (ResourceTypeId, NoOfResourcesRequired)
	SELECT [ResourceTypeId], NoOfResources
	FROM [dbo].[ServiceMetadata]
	WHERE [ServiceId] = @serviceID;

	IF EXISTS 
				(
				SELECT 1
				FROM [dbo].[ResourceTypes],#TempResourceTypeIDs2 
				WHERE [Id] = ResourceTypeId AND [shown] = 0 and [HasSchedual]=1
				)
				BEGIN
					INSERT INTO #TempInVisibleResourceTypeIDsHasSchedule (ResourceTypeId)
					SELECT [Id]
					FROM [dbo].[ResourceTypes],#TempResourceTypeIDs2
					WHERE [Id] = ResourceTypeId
					AND [shown] = 0
					AND [HasSchedual]=1
					SET @output = 1;
					
			END
			 ELSE
			 BEGIN
				SET @output = 0; -- Set the output variable value
				return @output;	
			END
	
			return @output; -- Return the output value
END";
            migrationBuilder.Sql(proc2);


            migrationBuilder.Sql(@"create proc proc3 @serviceID int			--shown 0 and schedule 0
with encryption 
as
BEGIN
declare  @output INT;
CREATE TABLE #TempResourceTypeIDs3 (
	ResourceTypeId INT,
	NoOfResourcesRequired int
);
CREATE TABLE #TempInVisibleResourceTypeIDsNoSchedule (
	ResourceTypeId INT,
);
	INSERT INTO #TempResourceTypeIDs3 (ResourceTypeId, NoOfResourcesRequired)
	SELECT [ResourceTypeId], NoOfResources
	FROM [dbo].[ServiceMetadata]
	WHERE [ServiceId] = @serviceID;

	IF EXISTS 
				(
				SELECT 1
				FROM [dbo].[ResourceTypes],#TempResourceTypeIDs3 
				WHERE [Id] = ResourceTypeId AND [shown] = 0 and [HasSchedual]=0
				)
				BEGIN
					INSERT INTO #TempInVisibleResourceTypeIDsNoSchedule (ResourceTypeId)
					SELECT [Id]
					FROM [dbo].[ResourceTypes],#TempResourceTypeIDs3
					WHERE [Id] = ResourceTypeId
					AND [shown] = 0
					AND [HasSchedual]=0
					SET @output = 1;
				END
			 ELSE
			 BEGIN
				SET @output = 0; -- Set the output variable value
				return @output;
			END
	
			return @output; -- Return the output value
END");

            migrationBuilder.Sql(@"create proc GetAvailableResourceForService  @date date,@serviceID int,@startTime time, @endTime time
as
BEGIN try --1
IF EXISTS (
	SELECT [ServiceId]
	FROM [dbo].[ServiceMetadata]
	WHERE [ServiceId] = @serviceID
)
BEGIN ---2
set nocount on ;
declare @resProc1 int 
declare @resProc2 int 
declare @resProc3 int 
EXEC @resProc1= proc1 @serviceID ;
EXEC @resProc2= proc2 @serviceID ;
EXEC @resProc3= proc3 @serviceID ;


CREATE TABLE #TempResourceTypeID (
	ResourceTypeId INT,
	NoOfResourcesRequired INT
);
	INSERT INTO #TempResourceTypeID (ResourceTypeId, NoOfResourcesRequired)
	SELECT [ResourceTypeId], NoOfResources
	FROM [dbo].[ServiceMetadata]
	WHERE [ServiceId] = @serviceID;

	--select * from #TempResourceTypeID
		--type1  has schedule and shown 
		--type2  has schedule and invisible 
		--type3  has Noschedule and invisible 
	IF 
	(@resProc1 =1 and @resProc2 =1 and @resProc3 =1)  --case if service has all types  first ,second ,third
	BEGIN  --3
		IF EXISTS			
				(--case1 shown 1 schedule 1
				SELECT 1
				FROM [dbo].[ResourceTypes],
				#TempResourceTypeID 
				WHERE [Id] = ResourceTypeId 
				AND [shown] = 1 
				AND [HasSchedual]=1
				)
				BEGIN  --4
						IF EXISTS 
						(--case 2  shown 0 schedule 1
						SELECT 1 
							 FROM [dbo].[ResourceTypes] RT,
							 #TempResourceTypeID TS,
							 [dbo].[ResourceSpecialCharacteristics]RSC,
							 [dbo].[Resource] R,
							 [dbo].[ScheduleItem] SI,
							 [dbo].[Schedule] S
							 WHERE RT.[Id] =TS.ResourceTypeId 
							 AND RSC.[ResourceID]=R.[Id]
							 AND R.[ResourceTypeId]=TS.ResourceTypeId
							 AND SI.[ScheduleId]=S.[ScheduleID]
							 AND S.[ResourceId]=RSC.[ResourceID]
							 AND SI.[ID]=RSC.[ScheduleID]
							 AND [shown] = 0 
							 AND [HasSchedual]=1
							 AND NoOfResourcesRequired<=RSC.AvailableCapacity 
							 AND SI.[Available]=1
							 AND @date =[Day]
							 AND SI.[StartTime]= @startTime 
							 AND SI.[EndTime]= @endTime
							 and S.[IsDeleted]=0
							 and SI.[IsDeleted]=0
						 )
						 BEGIN --5
							IF EXISTS 
							(--case 3   shown 0 schedule 0
							 SELECT 1 
							 FROM [dbo].[ResourceTypes] RT,#TempResourceTypeID TS,
							 [dbo].[ResourceSpecialCharacteristics]RSC,[dbo].[Resource] R
							 WHERE RT.[Id] =TS.ResourceTypeId 
							 and RSC.[ResourceID]=R.[Id]
							 and R.[ResourceTypeId]=TS.ResourceTypeId
							 AND [shown] = 0 
							 AND [HasSchedual]=0
							 AND NoOfResourcesRequired<=[AvailableCapacity])
							 BEGIN  --6
								select R.*
								from scheduleitem  as SI , 
								Schedule as S,
								[dbo].[Resource] R,
								#TempResourceTypeID,
								[dbo].[ResourceTypes] RT
								where 
								S.[ResourceId]=R.[Id]
								and  SI.scheduleId= S.scheduleId
								AND #TempResourceTypeID.[ResourceTypeId]=RT.[Id]
								AND R.[ResourceTypeId]=RT.[Id]
								AND SI.[Day]=@DATE
								AND SI.[StartTime]= @startTime 
								AND SI.[EndTime]= @endTime
								and available= 1 
								and S.[IsDeleted]=0
								and SI.[IsDeleted]=0
								AND [Shown]=1
								AND [HasSchedual]=1
								AND @date BETWEEN S.[FromDate] AND S.[ToDate]

						 END --6
				  END --5

			END --4
				
	 END --3
			 
		--type1  has schedule and shown 
		--type2  has schedule and invisible 
	
	ELSE IF (@resProc1 =1 and @resProc2 =1 and @resProc3=0 )--case if service has 2 types  first ,second 
	begin  --7
				IF EXISTS			
				(--case1 shown 1 schedule 1
				SELECT 1
				FROM [dbo].[ResourceTypes],
				#TempResourceTypeID 
				WHERE [Id] = ResourceTypeId 
				AND [shown] = 1 
				AND [HasSchedual]=1
				)
				BEGIN --8
						IF EXISTS 
						(--case 2  shown 0 schedule 1
						SELECT 1 
							 FROM [dbo].[ResourceTypes] RT,
							 #TempResourceTypeID TS,
							 [dbo].[ResourceSpecialCharacteristics]RSC,
							 [dbo].[Resource] R,
							 [dbo].[ScheduleItem] SI,
							 [dbo].[Schedule] S
							 WHERE RT.[Id] =TS.ResourceTypeId 
							 AND RSC.[ResourceID]=R.[Id]
							 AND R.[ResourceTypeId]=TS.ResourceTypeId
							 AND SI.[ScheduleId]=S.[ScheduleID]
							 AND S.[ResourceId]=RSC.[ResourceID]
							 AND SI.[ID]=RSC.[ScheduleID]
							 AND [shown] = 0 
							 AND [HasSchedual]=1
							 AND NoOfResourcesRequired<=RSC.AvailableCapacity 
							 AND SI.[Available]=1
							 AND SI.[Day]=@date
							 AND SI.[StartTime]= @startTime 
							 AND SI.[EndTime]= @endTime
							 and S.[IsDeleted]=0
							 and SI.[IsDeleted]=0
							 AND @date BETWEEN S.[FromDate] AND S.[ToDate]

						 )
						 
							 BEGIN --9
								select R.*
								from scheduleitem  as SI , 
								Schedule as S,
								[dbo].[Resource] R,
								#TempResourceTypeID,
								[dbo].[ResourceTypes] RT
								where 
								S.[ResourceId]=R.[Id]
								and  SI.scheduleId= S.scheduleId
								AND #TempResourceTypeID.[ResourceTypeId]=RT.[Id]
								AND R.[ResourceTypeId]=RT.[Id]
								AND SI.[Day]=@date
								AND SI.[StartTime]= @startTime 
								AND SI.[EndTime]= @endTime
								and available= 1 
								and S.[IsDeleted]=0
								and SI.[IsDeleted]=0
								AND [Shown]=1
								AND [HasSchedual]=1
								AND @date BETWEEN S.[FromDate] AND S.[ToDate]
							 END --9
				END  --8
		END--7

		--type1  has schedule and shown 
		--type3  has Noschedule and invisible 
	ELSE IF (@resProc1 =1and @resProc3 =1 and @resProc2=0 )--case if service has two types  first ,third
	begin  --10
				IF EXISTS			
				(--case1 shown 1 schedule 1
				SELECT 1
				FROM [dbo].[ResourceTypes],
				#TempResourceTypeID 
				WHERE [Id] = ResourceTypeId 
				AND [shown] = 1 
				AND [HasSchedual]=1
				)
				BEGIN --11
							IF EXISTS 
							(--case 3   shown 0 schedule 0
							 SELECT 1 
							 FROM [dbo].[ResourceTypes] RT,#TempResourceTypeID TS,
							 [dbo].[ResourceSpecialCharacteristics]RSC,[dbo].[Resource] R
							 WHERE RT.[Id] =TS.ResourceTypeId 
							 and RSC.[ResourceID]=R.[Id]
							 and R.[ResourceTypeId]=TS.ResourceTypeId
							 AND [shown] = 0 
							 AND [HasSchedual]=0
							 AND NoOfResourcesRequired<=RSC.[AvailableCapacity])
							 BEGIN --12
								select R.*
								from scheduleitem  as SI , 
								Schedule as S,
								[dbo].[Resource] R,
								#TempResourceTypeID,
								[dbo].[ResourceTypes] RT
								where 
								S.[ResourceId]=R.[Id]
								and  SI.scheduleId= S.scheduleId
								AND #TempResourceTypeID.[ResourceTypeId]=RT.[Id]
								AND R.[ResourceTypeId]=RT.[Id]
								AND SI.[Day]=@DATE
								AND SI.[StartTime]= @startTime 
								AND SI.[EndTime]= @endTime
								and available= 1 
								and S.[IsDeleted]=0
								and SI.[IsDeleted]=0
								AND [Shown]=1
								AND [HasSchedual]=1
								AND @date BETWEEN S.[FromDate] AND S.[ToDate]
							END --12
						END --11
				END --10
			ELSE IF (@resProc1 =1and @resProc2=0  and @resProc3=0)--case if service has one type  first type
			begin  --13
				IF EXISTS			
				(--case1 shown 1 schedule 1
				SELECT 1
				FROM [dbo].[ResourceTypes],
				#TempResourceTypeID 
				WHERE [Id] = ResourceTypeId 
				AND [shown] = 1 
				AND [HasSchedual]=1
				)
				BEGIN --14
							
						select R.*
						from scheduleitem  as SI , 
						Schedule as S,
						[dbo].[Resource] R,
						#TempResourceTypeID,
						[dbo].[ResourceTypes] RT
						where 
						S.[ResourceId]=R.[Id]
						and  SI.scheduleId= S.scheduleId
						AND #TempResourceTypeID.[ResourceTypeId]=RT.[Id]
						AND R.[ResourceTypeId]=RT.[Id]
						AND SI.[Day] =@date
						AND SI.[StartTime]= @startTime 
						AND SI.[EndTime]= @endTime
						and available= 1 
						and S.[IsDeleted]=0
						and SI.[IsDeleted]=0
						AND [Shown]=1
						AND [HasSchedual]=1
						AND @date BETWEEN S.[FromDate] AND S.[ToDate]
				END --14 
			END --13
		END  ---2
	END try-----1
	begin catch 
			select 0
	end catch");



            // Fill Client Booking Table
            migrationBuilder.Sql(@" Create proc FillClientBookingTable
 @UserID nvarchar(50) ,@date date,
 @servicesID int , @location nvarchar(20),
 @startTime time , @endTime time ,@output int output 
 with encryption 
as
--CREATE TABLE #TempResourceIds1 (
--    bookingID INT
--);

	begin try 

		insert into clientbookings	
		([UserId],[Date],
		[ServiceId],[Location],
		[Status],[StartTime],
		[EndTime],[TotalCost])

		values (@userid ,@date,@servicesID,@location,'Pending', @startTime, @endTime,0) 
		set @output = SCOPE_IDENTITY();  
	   --insert into #TempResourceIds1 values (@@identity)
	   --select * from #TempResourceIds1
	   end try 
	begin catch 
	end catch ");


            // Fill Booking item Table

            migrationBuilder.Sql(@"create proc FillBookingItemTableWithScheduleInvisible @BookingID int ,@output int output 
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
            migrationBuilder.Sql(@"create proc FillBookingItemTableNoScheduleInvisible @BookingID int, @output int output
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
            migrationBuilder.Sql(@"create proc FillBookingItemTableWithScheduleShown @BookingID int , @ResourceID int ,@output int output
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





            migrationBuilder.Sql(@"create PROCEDURE CheckServerTypeInBookingItem
	@serviceID INT,
	@OUTPUT INT OUTPUT
WITH ENCRYPTION
AS
BEGIN
	DECLARE @resProc1 INT;
	DECLARE @resProc2 INT;
	DECLARE @resProc3 INT;

	EXEC @resProc1 = proc1 @serviceID;
	EXEC @resProc2 = proc2 @serviceID;
	EXEC @resProc3 = proc3 @serviceID;

	IF (@resProc1 = 1 AND @resProc2 = 1 AND @resProc3 = 1) -- Case if service has all types: first, second, third
	BEGIN
		SET @OUTPUT = 1;
		SELECT @OUTPUT AS OutputResult;
		RETURN @OUTPUT;
	END
	ELSE IF (@resProc1 = 1 AND @resProc2 = 1 AND @resProc3 = 0) -- Case if service has 2 types: first, second
	BEGIN
		SET @OUTPUT = 2;
		SELECT @OUTPUT AS OutputResult;
		RETURN @OUTPUT;
	END
	ELSE IF (@resProc1 = 1 AND @resProc3 = 1 AND @resProc2 = 0) -- Case if service has two types: first, third
	BEGIN
		SET @OUTPUT = 3;
		SELECT @OUTPUT AS OutputResult;
		RETURN @OUTPUT;
	END
	ELSE IF (@resProc1 = 1 AND @resProc2 = 0 AND @resProc3 = 0) -- Case if service has one type: first type
	BEGIN
		SET @OUTPUT = 4;
		SELECT @OUTPUT AS OutputResult;
		RETURN @OUTPUT;
	END
	ELSE -- Case if service has one type: first type
	BEGIN
		SET @OUTPUT = 5;
		SELECT @OUTPUT AS OutputResult;
		RETURN @OUTPUT;
	END;
END;");


            // Change booking status
            migrationBuilder.Sql(@"Create proc SetBookingStatusConfiremed @bookingID int
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
		set [Status]= 'Confirmed'
		where [Id]=@bookingID
	end 
	else 
	return 0");
            migrationBuilder.Sql(@"Create PROCEDURE SetBookingStatuscompleted
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


            // total cost
            migrationBuilder.Sql(@"CREATE proc CalculateTotalCost @bookingID int
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

            // cancel pending booking
            migrationBuilder.Sql(@"create proc CancelPendingBooking @BookingID int 
with encryption 
as
	begin try 
	declare @time time 
	declare @date date
	declare @serviceID int


 CREATE TABLE #TempResourceTypeIDs (
        ResourceTypeId INT,
        NoOfResourcesRequired INT
    );

CREATE TABLE #TempALLRESOURCES (
        ResourceId INT,
    );
CREATE TABLE #TempResourceWithScheduleVisible (
        ResourceId INT,
    );
CREATE TABLE #TempResourceWithScheduleInvisible (
        ResourceId INT,
    );
CREATE TABLE #TempResourceNoScheduleInvisible (
        ResourceId INT,
    );

		IF EXISTS
		(
			select [Id], [StartTime]
			from [dbo].[ClientBookings]
			where [Id]=@BookingID
		)
		begin 
	
			select  @time=[StartTime],
			@date=[Date],
			@serviceID =[ServiceId]
			from [dbo].[ClientBookings]
			where [Id]=@BookingID

			INSERT INTO #TempResourceTypeIDs (ResourceTypeId, NoOfResourcesRequired)
			SELECT [ResourceTypeId], NoOfResources
			FROM [dbo].[ServiceMetadata]
			WHERE [ServiceId] = @serviceID;

			INSERT INTO #TempALLRESOURCES
			SELECT BI.[ResourceId] 
			FROM [dbo].[BookingItems] BI
			WHERE BI.[BookingId]=@BookingID
	

			insert into #TempResourceWithScheduleVisible 
			select R.[Id]
			from [dbo].[Resource] R,#TempResourceTypeIDs ,#TempALLRESOURCES,[dbo].[ResourceTypes] RT
			where R.[ResourceTypeId]=#TempResourceTypeIDs.ResourceTypeId
			and R.[Id]=#TempALLRESOURCES.ResourceId
			and RT.[Id]=#TempResourceTypeIDs.ResourceTypeId 
			and RT.[Shown]=1
			and RT.[HasSchedual]=1

			insert into #TempResourceWithScheduleInvisible 
			select R.[Id]
			from [dbo].[Resource] R,#TempResourceTypeIDs ,#TempALLRESOURCES,[dbo].[ResourceTypes] RT
			where R.[ResourceTypeId]=#TempResourceTypeIDs.ResourceTypeId
			and R.[Id]=#TempALLRESOURCES.ResourceId
			and RT.[Id]=#TempResourceTypeIDs.ResourceTypeId 
			and RT.[Shown]=0
			and RT.[HasSchedual]=1

			insert into #TempResourceNoScheduleInvisible 
			select R.[Id]
			from [dbo].[Resource] R,#TempResourceTypeIDs ,#TempALLRESOURCES,[dbo].[ResourceTypes] RT
			where R.[ResourceTypeId]=#TempResourceTypeIDs.ResourceTypeId
			and R.[Id]=#TempALLRESOURCES.ResourceId
			and RT.[Id]=#TempResourceTypeIDs.ResourceTypeId 
			and RT.[Shown]=0
			and RT.[HasSchedual]=0

		begin try 
			IF EXISTS
	     	(
				SELECT COUNT(*) FROM #TempResourceWithScheduleVisible
			)
			BEGIN 
				update  [dbo].[ScheduleItem] --1
				set [Available] =1 
				from [dbo].[ScheduleItem] as SI,[dbo].[Schedule]as S, #TempResourceWithScheduleVisible as TR
				where  S.[ScheduleId]=SI.[ScheduleID] 
				and S.[ResourceId]=TR.ResourceId 
				and SI.[Day]=@date 
				and SI.[StartTime]=@time
			END 

			IF EXISTS
	     	(
				SELECT COUNT(*) FROM #TempResourceWithScheduleInvisible
			)
			BEGIN TRY
				
				BEGIN TRANSACTION 
				 
					update  [dbo].[ScheduleItem] --2-1
					set [Available] =1 
					from [dbo].[ScheduleItem] as SI,[dbo].[Schedule]as S, #TempResourceWithScheduleInvisible as TR
					where  S.[ScheduleId]=SI.[ScheduleID] 
					and S.[ResourceId]=TR.ResourceId 
					and SI.[Day]=@date 
					and SI.[StartTime]=@time 

					update [dbo].[ResourceSpecialCharacteristics] --2-2
					set [AvailableCapacity]= [AvailableCapacity]+ RTids.NoOfResourcesRequired 
					from #TempResourceTypeIDs RTids ,[dbo].[ResourceTypes] RT,#TempResourceWithScheduleInvisible,
					[dbo].[ResourceSpecialCharacteristics] RSC,[dbo].[ScheduleItem] SI
					where  RTids.ResourceTypeId = RT.[Id] 
					AND #TempResourceWithScheduleInvisible.ResourceID=RSC.[ResourceID]
					AND RSC.[ScheduleID]=SI.[ID]
					AND  RT.[HasSchedual]=1
					AND RT.[Shown]=0
					commit;
				
			END TRY 
			BEGIN CATCH
						rollback;
			END CATCH 

			IF EXISTS
	     	(
				SELECT COUNT(*) FROM #TempResourceNoScheduleInvisible
			)
			BEGIN 
				update [dbo].[ResourceSpecialCharacteristics] --3
				set [AvailableCapacity]= [AvailableCapacity]+ RTids.NoOfResourcesRequired 
				from #TempResourceTypeIDs RTids ,[dbo].[ResourceTypes] RT,#TempResourceNoScheduleInvisible,
				[dbo].[ResourceSpecialCharacteristics] RSC
				where  RTids.ResourceTypeId = RT.[Id] 
				AND #TempResourceNoScheduleInvisible.ResourceID=RSC.[ResourceID]
				AND  RT.[HasSchedual]=0
				AND RT.[Shown]=0

			END 
		end try 
		begin catch
			select 0
		end catch

		begin try 
		BEGIN TRANSACTION;
			--update  [dbo].[BookingItems]
			--set [IsDeleted]=1
			--where [BookingId]=@BookingID

			update [dbo].[ClientBookings]
			set [Status]= 'Cancelled'
			where [Id] =@BookingID 
			commit;
		end try
		
		begin catch 
			rollback;
			select 0
		end catch

		 
	end 
		else 
		select 0
	end try 
	begin catch 
		select 0
	end catch");


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROC FillClientBookingTable");
            migrationBuilder.Sql("DROP PROC CheckServerTypeInBookingItem");

            migrationBuilder.Sql("DROP PROC FillBookingItemTableWithScheduleShown ");
            migrationBuilder.Sql("DROP PROC FillBookingItemTableNoScheduleInvisible ");
            migrationBuilder.Sql("DROP PROC FillBookingItemTableWithScheduleInvisible ");

            migrationBuilder.Sql("DROP PROC proc1");
            migrationBuilder.Sql("DROP PROC proc2");
            migrationBuilder.Sql("DROP PROC proc3");
            migrationBuilder.Sql("DROP PROC GetAvailableResourceForService");

            migrationBuilder.Sql("DROP PROC SetBookingStatusConfiremed");
            migrationBuilder.Sql("DROP PROC SetBookingStatuscompleted");

            migrationBuilder.Sql("DROP PROC CalculateTotalCost");

            migrationBuilder.Sql("DROP PROC CancelPendingBooking");

        }
    }
}
