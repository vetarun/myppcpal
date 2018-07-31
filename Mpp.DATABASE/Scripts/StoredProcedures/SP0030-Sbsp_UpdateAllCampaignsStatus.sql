IF EXISTS(
        SELECT*
        FROM sys.objects
        WHERE object_id = OBJECT_ID(N'[dbo].[Sbsp_UpdateAllCampaignsStatus]')

            AND type IN (
                N'P'
				, N'PC'
				)
		)
	DROP PROCEDURE[dbo].[Sbsp_UpdateAllCampaignsStatus]
GO 
Create PROCEDURE [dbo].[Sbsp_UpdateAllCampaignsStatus]
(
    @tblCamp CampaignType READONLY,
	@UserID int
)
AS
BEGIN
    DECLARE @RecordID bigint
	DECLARE @Status   int
    DECLARE @StartDate datetime
	DECLARE @EndDate datetime 
	DECLARE @LastActiveOn datetime
    DECLARE @CampCursor CURSOR 
    SET @CampCursor = CURSOR STATIC FOR SELECT RecordID, Status from @tblCamp
    OPEN @CampCursor
    FETCH NEXT FROM @CampCursor INTO @RecordID,@Status
    WHILE @@FETCH_STATUS = 0
    BEGIN    	
	SET @StartDate = (SELECT Top 1 bl.PlanStartDate FROM Billing bl WHERE bl.MppUserId=@UserID order by ID desc)
	SET @EndDate = (SELECT Top 1 bl.PlanEndDate FROM Billing bl WHERE bl.MppUserId=@UserID order by ID desc)
	
	SET @LastActiveOn = (SELECT LastActiveOn FROM Campaigns WHERE MppUserID=@UserID and RecordID=@RecordID)

	IF(@LastActiveOn NOT BETWEEN @StartDate AND @EndDate)
	BEGIN
		UPDATE Campaigns SET
			LastActiveOn = GETDATE(),
			LastDeactiveOn = GETDATE()
		WHERE MppUserID=@UserID and RecordID=@RecordID
	END
	ELSE
	BEGIN
		UPDATE Campaigns SET
			LastActiveOn=CASE WHEN (@status=1 AND Active=0 AND ((LastActiveOn IS NULL) OR DATEDIFF(DAY,LastActiveOn,LastDeactiveOn) < 1 )) THEN GETDATE()  ELSE LastActiveOn END,
			LastDeactiveOn=CASE WHEN (@status=0 AND Active=1 AND ((LastDeactiveOn IS NULL) OR DATEDIFF(DAY,LastActiveOn,GETDATE()) < 1 )) THEN GETDATE() ELSE LastDeactiveOn END
		WHERE MppUserID=@UserID and RecordID=@RecordID
	END

	UPDATE Campaigns SET
			Active = @Status,
			IncludeSku=(CASE WHEN @status=1 THEN 1
			WHEN @status=0 AND (DATEADD(DAY,1,LastActiveOn)<GETDATE() OR LastActiveOn IS NULL) THEN IncludeSku
			ELSE 0 END)
	WHERE MppUserID=@UserID and RecordID=@RecordID
	
	FETCH NEXT FROM @CampCursor INTO @RecordID,@Status
	END
	CLOSE @CampCursor
	DEALLOCATE @CampCursor  

    UPDATE MppUser SET IsSetFormula = 1, ModifiedOn=GetDate() where MppUserID= @UserID
END
GO