IF EXISTS(
        SELECT*
        FROM sys.objects
        WHERE object_id = OBJECT_ID(N'[dbo].[Sbsp_SnapShotManagement]')

            AND type IN (
                N'P'
				, N'PC'
				)
		)
	DROP PROCEDURE[dbo].[Sbsp_SnapShotManagement]
GO 
Create PROCEDURE [dbo].[Sbsp_SnapShotManagement]
(
    @date smalldatetime,
	@type int,
	@count int
)
AS
BEGIN
    DECLARE @ReqCount int 	    
	IF(@type = 2)
	BEGIN
		EXEC Sbsp_SetDailyReportDates @date
		SET @ReqCount = dbo.GetRequestCount(@count, 0, @date)	
		SELECT TOP (@ReqCount) r.ReportID,rt.RecordType,m.MppUserID,m.ProfileId,m.AccessToken,m.RefreshToken,m.TokenType,m.AccessTokenUpdatedOn,m.TokenExpiresIn from MppUser m JOIN Reports r on m.MppUserID = r.MppUserID JOIN ReportType rt on r.ReportID = rt.ReportID where m.ProfileAccess !=0 and m.IsArchive != 1 and rt.SnapStatus = 0 and rt.Snap_ReportId IS NULL and r.ReportDate = @date and rt.RecordType < 4 and rt.ApiAttempt < 3 order by m.MppUserID
    END
	ELSE IF(@type = 3)
	BEGIN
	    SET @ReqCount = dbo.GetRequestCount(@count, 1, @date)
 		SELECT TOP (@ReqCount) r.ReportID,rt.RecordType,m.MppUserID,rt.Snap_ReportId,m.ProfileId,m.AccessToken,m.RefreshToken,m.TokenType,m.AccessTokenUpdatedOn,m.TokenExpiresIn from MppUser m JOIN Reports r on m.MppUserID = r.MppUserID JOIN ReportType rt on r.ReportID = rt.ReportID where m.ProfileAccess !=0 and m.IsArchive != 1 and rt.SnapStatus = 1 and r.ReportDate = @date and rt.Snap_ReportId IS NOT NULL and DATEADD(mi,15,SnapReqDate)<=GETDATE() and rt.ApiAttempt < 3 order by m.MppUserID
	END
	ELSE
	BEGIN
	   SET @ReqCount = dbo.GetRequestCount(@count, 1, @date)
 	   SELECT TOP (@ReqCount) r.ReportID,rt.RecordType,m.MppUserID,rt.Snap_ReportId,m.ProfileId,m.AccessToken,m.RefreshToken,m.TokenType,m.AccessTokenUpdatedOn,m.TokenExpiresIn from MppUser m JOIN Reports r on m.MppUserID = r.MppUserID JOIN ReportType rt on r.ReportID = rt.ReportID where m.ProfileAccess !=0 and m.IsArchive != 1 and rt.SnapStatus < 0  and DATEADD(mi, 60 * rt.SnapAttempt, rt.SnapReqDate) BETWEEN GETDATE() AND DATEADD(d, 2, rt.SnapReqDate)
	   ORDER BY m.MppUserID
	END		
END
GO