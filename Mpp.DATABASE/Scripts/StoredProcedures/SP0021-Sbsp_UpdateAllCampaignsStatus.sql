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
CREATE PROCEDURE [dbo].[Sbsp_UpdateAllCampaignsStatus]  
(   
 @tblCamp CampaignType READONLY,  
 @UserID  int  
)  
AS  
BEGIN  
    DECLARE @RecordID bigint  
    DECLARE @Status   int   
    DECLARE @CampCursor CURSOR   
    SET @CampCursor = CURSOR STATIC FOR SELECT RecordID, Status from @tblCamp  
    OPEN @CampCursor  
    FETCH NEXT FROM @CampCursor INTO @RecordID,@Status  
    WHILE @@FETCH_STATUS = 0  
    BEGIN      
    UPDATE Campaigns set Active=@Status, ModifiedOn=GetDate()
	where MppUserID=@UserID and RecordID = @RecordID  
 FETCH NEXT FROM @CampCursor INTO @RecordID,@Status  
 END  
 CLOSE @CampCursor  
 DEALLOCATE @CampCursor    
    UPDATE MppUser SET IsSetFormula = 1, ModifiedOn=GetDate() where MppUserID= @UserID  
END
  