IF EXISTS(
        SELECT*
        FROM sys.objects
        WHERE object_id = OBJECT_ID(N'[dbo].[Sbsp_GetBestSrchTermCampaignsToCreate]')

            AND type IN (
                N'P'
				, N'PC'
				)
		)
	DROP PROCEDURE[dbo].[Sbsp_GetBestSrchTermCampaignsToCreate]
GO 
CREATE PROCEDURE Sbsp_GetBestSrchTermCampaignsToCreate 
AS
BEGIN
	SET NOCOUNT ON;
	SELECT TOP 10 c.Name+'-MPP' CampaignName,c.DailyBudget,bst.MppUserID FROM BestSearchTermRequest bst 
	JOIN Campaigns c ON c.RecordID=bst.CampaignId
	WHERE bst.Status=0

	SELECT DISTINCT MppUser.MppUserID,ProfileId, AccessToken, RefreshToken, TokenType, AccessTokenUpdatedOn, TokenExpiresIn FROM MppUser
	JOIN  BestSearchTermRequest bst ON bst.MppUserID=MppUser.MppUserID
    WHERE  ProfileAccess=1

END
GO
