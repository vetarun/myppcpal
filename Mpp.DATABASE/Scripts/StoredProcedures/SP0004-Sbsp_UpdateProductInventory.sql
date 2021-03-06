
IF EXISTS(
        SELECT*
        FROM sys.objects
        WHERE object_id = OBJECT_ID(N'[dbo].[Sbsp_UpdateProductInventory]')

            AND type IN (
                N'P'
				, N'PC'
				)
		)
	DROP PROCEDURE[dbo].[Sbsp_UpdateProductInventory]
GO 

CREATE PROCEDURE [dbo].[Sbsp_UpdateProductInventory]
(	
	@tblProducts ProductInventoryType READONLY,
	@UserID  int,
	@ReportId int,
	@RecordType int,
	@Status     int,
	@Email varchar(100) out 
)  
AS
BEGIN
BEGIN TRANSACTION
SET XACT_ABORT ON
      SET @Email = ''
      MERGE INTO ProductsInventory i1
      USING @tblProducts i2
      ON i1.RecordID=i2.RecordID and i1.RecordType = @RecordType and i1.MppUserID=@UserID and i1.ReportID = @ReportId
      WHEN MATCHED THEN
      UPDATE SET i1.Impressions=i2.Impressions,i1.Clicks=i2.Clicks,i1.Spend=i2.Spend, i1.Orders=i2.Orders,i1.Sales=i2.Sales,i1.ModifiedOn=GetDate()
      WHEN NOT MATCHED THEN 
      INSERT VALUES(@UserID,@ReportID, i2.RecordID, @RecordType,i2.Impressions,i2.Clicks,i2.Spend,i2.Orders,i2.Sales,null,getdate());

	  IF(@Status = 5)
	  BEGIN
	    UPDATE ReportType SET ReportStatus = 2, RefreshStatus = 2, ModifiedOn=GetDate(),RefStatusCount=RefStatusCount+1 ,RefreshedOn=GetDate() 
		WHERE ReportId = @ReportId and RecordType=@RecordType --and cast(RefreshedOn as date ) < cast (getDate() as date )
	    END
	  ELSE
	  BEGIN
	    UPDATE ReportType SET ReportStatus = 2, ModifiedOn=GetDate() WHERE ReportId = @ReportId and RecordType=@RecordType
	  END

	  IF EXISTS(SELECT 1 FROM MppUser WHERE DataImportAlert = 0 and MppUserID = @UserID)
	  BEGIN
	    DECLARE @Count int 
		SET @Count = (SELECT count(*) FROM ReportType WHERE ReportId IN (SELECT ReportId FROM Reports WHERE MppUserID=@UserID) and ReportStatus=2 and RecordType=1 )
		IF(@Count > 55)
		BEGIN
		  SET @Email = (SELECT Email FROM MppUser WHERE MppUserID = @UserID)
		END
	  END
	  SELECT @Email Email
COMMIT
END
