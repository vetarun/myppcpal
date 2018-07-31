
IF EXISTS(
        SELECT*
        FROM sys.objects
        WHERE object_id = OBJECT_ID(N'[dbo].[Sbsp_SetReportDates]')

            AND type IN (
                N'P'
				, N'PC'
				)
		)
	DROP PROCEDURE[dbo].[Sbsp_SetReportDates]
GO  
Create PROCEDURE Sbsp_SetReportDates
(
	@enddate   smalldatetime
)
AS
BEGIN
    BEGIN TRANSACTION
	DECLARE @StartDate smalldatetime 
	SET @StartDate = DATEADD(d, -56, @enddate)
    
	DECLARE @MppUserID int
	DECLARE @UserID CURSOR 
    SET @UserID = CURSOR STATIC FOR SELECT MppUserID from MppUser where ProfileAccess !=0
    OPEN @UserID
    FETCH NEXT FROM @UserID INTO @MppUserID
    WHILE @@FETCH_STATUS = 0
    BEGIN    
		INSERT INTO Reports(MppUserID,ReportDate)
				SELECT @MppUserID,date from dbo.fGetListOfDates(@StartDate, @enddate)
				WHERE date NOT IN (SELECT ReportDate FROM Reports where MppUserID = @MppUserID)
	FETCH NEXT FROM @UserID INTO @MppUserID
    END
	CLOSE @UserID
	DEALLOCATE @UserID
	
	INSERT INTO ReportType(ReportID,RecordType)
	SELECT ReportID, RecordTypeId from Reports CROSS JOIN RecordType where ReportID NOT IN (Select ReportID from ReportType)	 
	COMMIT
END
GO


