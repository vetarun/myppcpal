IF EXISTS(
        SELECT*
        FROM sys.objects
        WHERE object_id = OBJECT_ID(N'[dbo].[Sbsp_UpdateKeywords]')

            AND type IN (
                N'P'
				, N'PC'
				) 
		)
	DROP PROCEDURE[dbo].[Sbsp_UpdateKeywords]
GO 
CREATE PROCEDURE [dbo].[Sbsp_UpdateKeywords]
(	
	@tblKeywords KeywordType READONLY,
	@UserID  int,
	@ReportId int,
    @RecordType int
)  
AS
BEGIN
BEGIN TRANSACTION
SET XACT_ABORT ON
      MERGE INTO Keywords k1
      USING @tblKeywords k2
      ON k1.RecordID=k2.RecordID and k1.CampaignID=k2.CampaignID and k1.AdGroupID=k2.AdGroupID and k1.MppUserID=@UserID
      WHEN MATCHED THEN
      UPDATE SET k1.Keyword = k2.Keyword, k1.DailyBudget = k2.DailyBudget,k1.Bid=k2.Bid,k1.MatchType=k2.MatchType,k1.Status=k2.Status,k1.ModifiedOn=GetDate()
      WHEN NOT MATCHED THEN 
      INSERT VALUES(k2.RecordID,@UserID,k2.CampaignID,k2.AdGroupID,k2.DailyBudget,k2.Bid,k2.Keyword,k2.MatchType,k2.Status,null,getdate(),null);

	  Update ReportType set SnapStatus = 2, ModifiedOn=GetDate() where ReportId = @ReportId and RecordType=@RecordType
COMMIT
END