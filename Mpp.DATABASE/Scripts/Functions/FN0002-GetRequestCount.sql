IF EXISTS(SELECT 1 FROM SYS.OBJECTS WHERE Name = N'GetRequestCount')
BEGIN
	DROP FUNCTION GetRequestCount
END
GO
CREATE FUNCTION [dbo].[GetRequestCount]
(
 @maxCount int,
 @type int,
 @date smalldatetime
)
RETURNS INT
AS 
BEGIN
    DECLARE @StartDate smalldatetime
	DECLARE @CurrentDate smalldatetime
	DECLARE @Sp_NotSet int
	DECLARE @Sp_Set int
    DECLARE @Iy_Old_NotSet int
    DECLARE @Iy_Old_Set int
    DECLARE @Iy_Cur_NotSet int
    DECLARE @Iy_Cur_Set int
    DECLARE @Iy_Ref_NotSet int
    DECLARE @Iy_Ref_Set int
	DECLARE @TotalCount int = 0
	DECLARE @RequestCount int = 0
	DECLARE @DefaultCount int 

	SET @CurrentDate = DATEADD(d,3,@date)

	SET @StartDate = DATEADD(d,-56,@date)

	SET @Sp_NotSet = (SELECT count(*) from MppUser m JOIN Reports r on m.MppUserID = r.MppUserID JOIN ReportType rt on r.ReportID = rt.ReportID where m.ProfileAccess !=0 and m.IsArchive != 1 and rt.SnapStatus = 0 and rt.Snap_ReportId IS NULL and r.ReportDate = @date and rt.RecordType < 4 and rt.ApiAttempt < 3)

	SET @Sp_Set = (SELECT count(*) from MppUser m JOIN Reports r on m.MppUserID = r.MppUserID JOIN ReportType rt on r.ReportID = rt.ReportID where m.ProfileAccess !=0 and m.IsArchive != 1 and rt.SnapStatus = 1 and r.ReportDate = @date and rt.Snap_ReportId IS NOT NULL and DATEADD(mi,15,SnapReqDate)<=GETDATE() and rt.ApiAttempt < 3)

	SET @Iy_Old_NotSet = (SELECT count(*) from MppUser m JOIN Reports r on m.MppUserID = r.MppUserID JOIN ReportType rt on r.ReportID = rt.ReportID 
	WHERE m.ProfileAccess !=0 and m.IsArchive != 1 and rt.Amz_ReportId IS NULL and r.ReportDate >= @StartDate and r.ReportDate < @date and rt.ReportStatus = 0 and rt.RefreshStatus = 0 and rt.ApiAttempt < 3)

 	SET @Iy_Old_Set = (SELECT count(*) from MppUser m JOIN Reports r on m.MppUserID = r.MppUserID JOIN ReportType rt on r.ReportID = rt.ReportID 
	WHERE m.ProfileAccess !=0 and m.IsArchive != 1 and rt.ReportStatus = 1 and rt.Amz_ReportId IS NOT NULL and r.ReportDate < @date and DATEADD(mi,15,ReportReqDate) <= GETDATE() and rt.ApiAttempt < 3)

	SET @Iy_Cur_NotSet = (SELECT count(*) from MppUser m JOIN Reports r on m.MppUserID = r.MppUserID JOIN ReportType rt on r.ReportID = rt.ReportID 
	WHERE m.ProfileAccess !=0 and m.IsArchive != 1 and rt.Amz_ReportId IS NULL and r.ReportDate = @date and rt.ReportStatus = 0 and rt.ApiAttempt < 3)

 	SET @Iy_Cur_Set = (SELECT count(*) from MppUser m JOIN Reports r on m.MppUserID = r.MppUserID JOIN ReportType rt on r.ReportID = rt.ReportID 
	WHERE m.ProfileAccess !=0 and m.IsArchive != 1 and rt.ReportStatus = 1 and rt.Amz_ReportId IS NOT NULL and r.ReportDate = @date and DATEADD(mi,15,ReportReqDate) <= GETDATE() and rt.ApiAttempt < 3)

	SET @Iy_Ref_NotSet = (SELECT count(*) from MppUser m JOIN Reports r on m.MppUserID = r.MppUserID JOIN ReportType rt on r.ReportID = rt.ReportID 
	WHERE m.ProfileAccess !=0 and rt.RefreshStatus = 0 and rt.ReportStatus = 2 and m.StartDate != @CurrentDate  and r.ReportDate = DATEADD(d,-5,@date) 	 
	  and (  r.ReportDate =  DATEADD(d,-14,@date) or r.ReportDate =  DATEADD(d,-27,@date) or 
		 r.ReportDate BETWEEN DATEADD(d,-7,@date) AND DATEADD(d,-1,@date) ) 
		  )

	SET @Iy_Ref_Set = (SELECT count(*) from MppUser m JOIN Reports r on m.MppUserID = r.MppUserID JOIN ReportType rt on r.ReportID = rt.ReportID 
	WHERE m.ProfileAccess !=0 and rt.RefreshStatus = 1 and rt.Amz_ReportId IS NOT NULL and DATEADD(mi,15,ReportReqDate) <= GETDATE())

	IF(@maxCount = 0)
		SET @maxCount = 160

    SET @DefaultCount = 20

	--Snap Shot Reports
	IF(@type != 0)
	BEGIN    
		IF (@Sp_NotSet > @DefaultCount)
			SET @TotalCount += @DefaultCount;
		ELSE
			SET @TotalCount += @Sp_NotSet;
	END

	IF(@type != 1)
	BEGIN    
		IF (@Sp_Set > @DefaultCount)
			SET @TotalCount += @DefaultCount;
		ELSE
			SET @TotalCount += @Sp_Set;
	END

	-- Inventory Reports    
	IF(@type != 2)
	BEGIN 
		IF (@Iy_Old_NotSet > @DefaultCount)
			SET @TotalCount += @DefaultCount;
		ELSE
			SET @Totalcount += @Iy_Old_NotSet;
	END
	
	IF(@type != 3)
	BEGIN 
		IF (@Iy_Old_Set > @DefaultCount)
			SET @TotalCount += @DefaultCount;
		ELSE
			SET @TotalCount += @Iy_Old_Set;
    END

	IF(@type != 4)
	BEGIN
		IF (@Iy_Cur_NotSet > @DefaultCount)
			SET @TotalCount += @DefaultCount;
		ELSE
			SET @TotalCount += @Iy_Cur_NotSet;
	END

	IF(@type != 5)
	BEGIN
		IF (@Iy_Cur_Set > @DefaultCount)
			SET @TotalCount += @DefaultCount;
		ELSE
			SET @TotalCount += @Iy_Cur_Set;
	END

	IF (CAST(GETDATE() as time) >= '03:00:00' AND CAST(GETDATE() as time) < '19:59:00')
	BEGIN
		IF(@type != 6)
		BEGIN
			IF (@Iy_Ref_NotSet > @DefaultCount)
				SET @TotalCount += @Iy_Ref_NotSet;
			ELSE
				SET @TotalCount += @DefaultCount;
		END
		
		IF(@type != 7)
		BEGIN
			IF (@Iy_Ref_Set > @DefaultCount)
				SET @TotalCount += @DefaultCount;
			ELSE
				SET @TotalCount += @Iy_Ref_Set;
		END
    END

    SET @RequestCount = @maxCount - @TotalCount;
	
	IF(@type = 8)
	BEGIN
		IF(@RequestCount < @maxCount)
		BEGIN
		  SET @RequestCount = @maxCount - @RequestCount;
		END
	END

	IF(@RequestCount <= 0)
	BEGIN
	    SET @RequestCount = @maxCount;
	END

	RETURN @RequestCount
END
