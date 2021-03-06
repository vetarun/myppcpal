IF EXISTS(
        SELECT*
        FROM sys.objects
        WHERE object_id = OBJECT_ID(N'[dbo].[Sbsp_GetUserPlan]')

            AND type IN (
                N'P'
				, N'PC'
				)
		)
	DROP PROCEDURE[dbo].[Sbsp_GetUserPlan]
GO  

CREATE PROCEDURE [dbo].[Sbsp_GetUserPlan] 
(	
	@UserID	int
)
AS
BEGIN
     DECLARE @sku int 
	 DECLARE @keyword int

     SELECT Top 1 bl.PlanEndDate
	 FROM MppUser u left outer join Billing bl on u.MppUserID = bl.MppUserId
	 where u.MppUserID=@UserID order by bl.ID desc

     SET  @sku = ISNULL((SELECT COUNT(DISTINCT a.sku) sku from Campaigns c INNER JOIN
	                         Ads a ON c.RecordID = a.CampaignID
							 WHERE c.IncludeSku = 1 and a.Status = 1 and c.MppUserID = @userId), 0)
	
	 SET @keyword = ISNULL((SELECT count(*) from Keywords where MppUserID=@UserID), 0)

	 SELECT @sku SKU, @keyword Keyword
END

