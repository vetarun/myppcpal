IF EXISTS(
        SELECT*
        FROM sys.objects
        WHERE object_id = OBJECT_ID(N'[dbo].[Sbsp_GetUserSku]')

            AND type IN (
                N'P'
				, N'PC'
				) 
		)
	DROP PROCEDURE[dbo].[Sbsp_GetUserSku]
GO 
CREATE PROCEDURE [dbo].[Sbsp_GetUserSku] 
@userId VARCHAR(200)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT COUNT(DISTINCT a.sku) sku from Campaigns c INNER JOIN
	                         Ads a ON c.RecordID = a.CampaignID
							 WHERE c.IncludeSku = 1 and a.Status = 1 and c.MppUserID = @userId
END
