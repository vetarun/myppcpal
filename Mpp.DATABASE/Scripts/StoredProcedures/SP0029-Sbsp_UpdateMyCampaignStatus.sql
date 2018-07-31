IF EXISTS(
        SELECT*
        FROM sys.objects
        WHERE object_id = OBJECT_ID(N'[dbo].[Sbsp_UpdateMyCampaignStatus]')

            AND type IN (
                N'P'
				, N'PC'
				) 
		)
	DROP PROCEDURE[dbo].[Sbsp_UpdateMyCampaignStatus]
	GO
CREATE PROCEDURE [dbo].[Sbsp_UpdateMyCampaignStatus] 
(  
 @UserID int,
 @CampId bigint,
 @Status int,  
 @ModifiedOn smalldatetime  
)  
AS  
BEGIN        
	DECLARE @StartDate datetime
	DECLARE @EndDate datetime
    DECLARE @LastActiveOn datetime
	SET @StartDate = (SELECT Top 1 bl.PlanStartDate FROM Billing bl WHERE bl.MppUserId=@UserID order by ID desc)
	SET @EndDate = (SELECT Top 1 bl.PlanEndDate FROM Billing bl WHERE bl.MppUserId=@UserID order by ID desc)
	
	SET @LastActiveOn = (SELECT LastActiveOn FROM Campaigns WHERE MppUserID=@UserID and RecordID=@CampId)

	IF(@LastActiveOn NOT BETWEEN @StartDate AND @EndDate)
	BEGIN
		UPDATE Campaigns SET
			LastActiveOn = GETDATE(),
			LastDeactiveOn = GETDATE()
		WHERE MppUserID=@UserID and RecordID=@CampId
	END
	ELSE
	BEGIN
		UPDATE Campaigns SET
			LastActiveOn=CASE WHEN (@status=1 AND Active=0 AND ((LastActiveOn IS NULL) OR DATEDIFF(DAY,LastActiveOn,LastDeactiveOn) < 1 )) THEN GETDATE()  ELSE LastActiveOn END,
			LastDeactiveOn=CASE WHEN (@status=0 AND Active=1 AND ((LastDeactiveOn IS NULL) OR DATEDIFF(DAY,LastActiveOn,GETDATE()) < 1 )) THEN GETDATE() ELSE LastDeactiveOn END
		WHERE MppUserID=@UserID and RecordID=@CampId
	END

	UPDATE Campaigns SET
			Active = @Status,
			IncludeSku=(CASE WHEN @status=1 THEN 1
			WHEN @status=0 AND (DATEADD(DAY,1,LastActiveOn)<GETDATE() OR LastActiveOn IS NULL) THEN IncludeSku
			ELSE 0 END)
	WHERE MppUserID=@UserID and RecordID=@CampId

	UPDATE MppUser SET IsSetFormula = 1, ModifiedOn=GetDate() where MppUserID= @UserID    
END  