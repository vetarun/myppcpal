IF EXISTS(
        SELECT*
        FROM sys.objects
        WHERE object_id = OBJECT_ID(N'[dbo].[Sbsp_ReportManagement]')

            AND type IN (
                N'P'
				, N'PC'
				)
		)
	DROP PROCEDURE[dbo].[Sbsp_ReportManagement]
GO  
CREATE PROCEDURE [dbo].[Sbsp_ReportManagement] 
(
    @date smalldatetime,
	@type int,
	@count int
)
AS
BEGIN
    DECLARE @StartDate smalldatetime
	DECLARE @CurrentDate smalldatetime
	DECLARE @ReqCount int
	SET @CurrentDate = DATEADD(d,3,@date)
	
	IF(@type = 0) /* NOT SET - Reports Not Being Processed Before End Date*/
	BEGIN
		SET @StartDate = DATEADD(d,-56,@date)
		EXEC Sbsp_SetDailyReportDates @date	

		SET @ReqCount = dbo.GetRequestCount(@count, 2, @date)	  
		
		SELECT TOP (@ReqCount) r.ReportID,r.ReportDate,rt.RecordType,m.MppUserID,m.ProfileId,m.AccessToken,m.RefreshToken,m.TokenType,m.AccessTokenUpdatedOn,m.TokenExpiresIn from MppUser m JOIN Reports r on m.MppUserID = r.MppUserID JOIN ReportType rt on r.ReportID = rt.ReportID 
		WHERE m.ProfileAccess !=0 and m.IsArchive != 1 and rt.Amz_ReportId IS NULL and r.ReportDate >= @StartDate and r.ReportDate < @date and rt.ReportStatus = 0 and rt.RefreshStatus = 0 and rt.ApiAttempt < 3 order by m.MppUserID
    END
	ELSE IF(@type = 1) /* REQUESTED - Reports Not Being Processed Before End Date */
	BEGIN
	    SET @ReqCount = dbo.GetRequestCount(@count, 3, @date)	
		  
 		SELECT TOP (@ReqCount) r.ReportID,rt.RecordType,m.MppUserID,rt.Amz_ReportId,m.ProfileId,m.AccessToken,m.RefreshToken,m.TokenType,m.AccessTokenUpdatedOn,m.TokenExpiresIn from MppUser m JOIN Reports r on m.MppUserID = r.MppUserID JOIN ReportType rt on r.ReportID = rt.ReportID 
		WHERE m.ProfileAccess !=0 and m.IsArchive != 1 and rt.ReportStatus = 1 and rt.Amz_ReportId IS NOT NULL and r.ReportDate < @date and DATEADD(mi,15,ReportReqDate) <= GETDATE() and rt.ApiAttempt < 3 order by m.MppUserID
	END
	ELSE IF(@type = 2) /* NOT SET - Current Date */
	BEGIN
	    SET @ReqCount = dbo.GetRequestCount(@count, 4, @date)  

		SELECT TOP (@ReqCount) r.ReportID,r.ReportDate,rt.RecordType,m.MppUserID,m.ProfileId,m.AccessToken,m.RefreshToken,m.TokenType,m.AccessTokenUpdatedOn,m.TokenExpiresIn from MppUser m JOIN Reports r on m.MppUserID = r.MppUserID JOIN ReportType rt on r.ReportID = rt.ReportID 
		WHERE m.ProfileAccess !=0 and m.IsArchive != 1 and rt.Amz_ReportId IS NULL and r.ReportDate = @date and rt.ReportStatus = 0 and rt.ApiAttempt < 3 order by m.MppUserID
    END
	ELSE IF(@type = 3) /* REQUESTED - Current Date*/
	BEGIN
	    SET @ReqCount = dbo.GetRequestCount(@count, 5, @date)

 		SELECT TOP (@ReqCount) r.ReportID,rt.RecordType,m.MppUserID,rt.Amz_ReportId,m.ProfileId,m.AccessToken,m.RefreshToken,m.TokenType,m.AccessTokenUpdatedOn,m.TokenExpiresIn from MppUser m JOIN Reports r on m.MppUserID = r.MppUserID JOIN ReportType rt on r.ReportID = rt.ReportID 
		WHERE m.ProfileAccess !=0 and m.IsArchive != 1 and rt.ReportStatus = 1 and rt.Amz_ReportId IS NOT NULL and r.ReportDate = @date and DATEADD(mi,15,ReportReqDate) <= GETDATE() and rt.ApiAttempt < 3 order by m.MppUserID
	END
	ELSE IF(@type = 4) /* NOT SET -  REFRESH  */
	BEGIN
	      SET @ReqCount = dbo.GetRequestCount(@count, 6, @date) 
		
	    SELECT TOP (@ReqCount) r.ReportID,r.ReportDate,rt.RecordType,m.MppUserID,m.ProfileId,m.AccessToken,m.RefreshToken,m.TokenType,
        m.AccessTokenUpdatedOn,m.TokenExpiresIn FROM MppUser m JOIN Reports r ON m.MppUserID = r.MppUserID JOIN
        ReportType rt ON r.ReportID = rt.ReportID WHERE m.ProfileAccess !=0 and rt.RefreshStatus = 0 and rt.ReportStatus = 2 and m.StartDate !=  @CurrentDate  
		and CAST(r.ReportDate AS DATE)=CAST(@date AS DATE) and (rt.RefreshedOn IS NULL OR CAST(rt.RefreshedOn as DATE) !=CAST(GETDATE() AS DATE))
		UNION 
	    SELECT TOP (@ReqCount)  r.ReportID,r.ReportDate,rt.RecordType,m.MppUserID,m.ProfileId,m.AccessToken,m.RefreshToken,m.TokenType,
        m.AccessTokenUpdatedOn,m.TokenExpiresIn FROM MppUser m JOIN Reports r ON m.MppUserID = r.MppUserID JOIN
        ReportType rt ON r.ReportID = rt.ReportID WHERE m.ProfileAccess !=0 and rt.ReportStatus = 2 and m.StartDate !=  @CurrentDate 
		and (rt.RefreshedOn IS NULL OR CAST(rt.RefreshedOn as DATE) !=CAST(GETDATE() AS DATE)) 
		and (CAST(r.ReportDate AS DATE) = CAST(DATEADD(d,-14,@date) AS DATE) or CAST(r.ReportDate AS DATE) = CAST(DATEADD(d,-27,@date) AS DATE) OR 
		CAST(r.ReportDate AS DATE) BETWEEN CAST(DATEADD(d,-6,@date) AS DATE) AND CAST(DATEADD(d,0,@date) AS DATE))
		ORDER BY m.MppUserID
	END
	ELSE IF(@type = 5) /* REQUESTED - REFRESH */
	BEGIN
	    SET @ReqCount = dbo.GetRequestCount(@count, 7, @date)  

	    SELECT TOP (@ReqCount) r.ReportID,ReportReqDate,rt.RecordType,m.MppUserID,rt.Amz_ReportId,m.ProfileId,m.AccessToken,m.RefreshToken,m.TokenType,m.AccessTokenUpdatedOn,m.TokenExpiresIn from
		 MppUser m 
		 JOIN Reports r on m.MppUserID = r.MppUserID 
		 JOIN ReportType rt on r.ReportID = rt.ReportID 
		WHERE m.ProfileAccess !=0 and rt.RefreshStatus = 1 and rt.Amz_ReportId IS NOT NULL and DATEADD(mi,15,ReportReqDate) <= GETDATE() and rt.ApiAttempt < 3 order by m.MppUserID		 
	END
	ELSE IF(@type = 6) /* FAILED */
	BEGIN
	    SET @ReqCount = dbo.GetRequestCount(0, 8, @date) 

        SELECT TOP (@ReqCount) r.ReportID,r.ReportDate,rt.RecordType,m.MppUserID,m.ProfileId,m.AccessToken,m.RefreshToken,m.TokenType,m.AccessTokenUpdatedOn,m.TokenExpiresIn from MppUser m JOIN Reports r on m.MppUserID = r.MppUserID JOIN ReportType rt on r.ReportID = rt.ReportID 
		WHERE m.ProfileAccess !=0 and (rt.ReportStatus <0 or rt.RefreshStatus < 0) and DATEADD(mi, 60 * rt.InventoryAttempt, rt.ReportReqDate) BETWEEN GETDATE() AND DATEADD(d, 2, rt.ReportReqDate) ORDER BY m.MppUserID
	END
END