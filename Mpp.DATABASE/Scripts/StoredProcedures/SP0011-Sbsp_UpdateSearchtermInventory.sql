
IF EXISTS(
        SELECT*
        FROM sys.objects
        WHERE object_id = OBJECT_ID(N'[dbo].[Sbsp_UpdateSearchtermInventory]')

            AND type IN (
                N'P'
				, N'PC'
				)
		)
	DROP PROCEDURE[dbo].[Sbsp_UpdateSearchtermInventory]
GO  
Create PROCEDURE Sbsp_UpdateSearchtermInventory
(	
	@tblSearchterm SearchtermType READONLY,
	@UserID  int,
	@ReportId int,
	@RecordType int,
	@Status     int
)  
AS
BEGIN
BEGIN TRANSACTION
SET XACT_ABORT ON
      IF NOT EXISTS(select 1 from SearchtermInventory where MppUserID != @UserID and ReportID = @ReportId and RecordID IN (SELECT RecordID FROM @tblSearchterm))
	  BEGIN
      MERGE INTO SearchtermInventory i1
      USING @tblSearchterm i2
      ON i1.RecordID=i2.RecordID and i1.MppUserID=@UserID and i1.ReportID = @ReportId and i1.Query=i2.Query
      WHEN MATCHED THEN
      UPDATE SET i1.Impressions=i2.Impressions,i1.Clicks=i2.Clicks,i1.Spend=i2.Spend, i1.Orders=i2.Orders,i1.Sales=i2.Sales,i1.ModifiedOn=GetDate()
      WHEN NOT MATCHED THEN 
      INSERT VALUES(@UserID,@ReportID, i2.RecordID,i2.Query,i2.Impressions,i2.Clicks,i2.Spend,i2.Orders,i2.Sales,0,null,GETDATE());

	  IF(@Status = 5)
	  BEGIN
	    UPDATE ReportType set ReportStatus = 2, RefreshStatus = 2,RefStatusCount=RefStatusCount+1,RefreshedOn=GETDATE(),ModifiedOn=GETDATE()
		WHERE  ReportId = @ReportId and RecordType=@RecordType
	  END
	  ELSE
	  BEGIN
		UPDATE ReportType set ReportStatus = 2, ModifiedOn=GETDATE() WHERE  ReportId = @ReportId and RecordType=@RecordType
	  END
END
COMMIT
END
GO


