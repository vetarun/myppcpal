IF EXISTS(
        SELECT*
        FROM sys.objects
        WHERE object_id = OBJECT_ID(N'[dbo].[Sbsp_UpdateBestSrchTerm]')

            AND type IN (
                N'P'
				, N'PC'
				)
		)
	DROP PROCEDURE[dbo].[Sbsp_UpdateBestSrchTerm]
GO  
CREATE PROCEDURE [dbo].[Sbsp_UpdateBestSrchTerm]  
(   
@campId BIGINT ,
@newCampId BIGINT,
@userId INT
)  
AS  
BEGIN  
  UPDATE BestSearchTermRequest SET Status=1,NewCampaignId=@newCampId
  WHERE CampaignId=@campId AND MppUserId=@userId
END
  