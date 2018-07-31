IF EXISTS(
        SELECT*
        FROM sys.objects
        WHERE object_id = OBJECT_ID(N'[dbo].[Sbsp_UpdateAds]')

            AND type IN (
                N'P'
				, N'PC'
				) 
		)
	DROP PROCEDURE[dbo].[Sbsp_UpdateAds]
GO 
CREATE PROCEDURE Sbsp_UpdateAds
(	
	@tblAds AdType READONLY,
	@UserID  int,
	@Date smalldatetime
)  
AS
BEGIN
BEGIN TRANSACTION
SET XACT_ABORT ON
      MERGE INTO Ads a1
      USING @tblAds a2
      ON a1.RecordID=a2.RecordID and a1.CampaignID=a2.CampaignID and a1.AdGroupID=a2.AdGroupID and a1.MppUserID=@UserID
	  WHEN MATCHED THEN
      UPDATE SET a1.Status = 1,a1.AdDate=@Date,a1.ModifiedOn=GETDATE()
      WHEN NOT MATCHED THEN 
      INSERT VALUES(a2.RecordID,@UserID,a2.CampaignID,a2.AdGroupID,a2.Sku,@Date, 1, null, GETDATE());

	  UPDATE Ads SET Status = 0 where AdDate < @Date and MppUserID=@UserID

COMMIT
END
GO