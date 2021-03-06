IF EXISTS(
        SELECT*
        FROM sys.objects
        WHERE object_id = OBJECT_ID(N'[dbo].[Sbsp_UpdateInventoryStatus]')

            AND type IN (
                N'P'
				, N'PC'
				)
		)
	DROP PROCEDURE[dbo].[Sbsp_UpdateInventoryStatus]
GO 


CREATE PROCEDURE [dbo].[Sbsp_UpdateInventoryStatus]
(
    @ReportId int,
	@Amz_ReportId varchar(max),
	@Status int,
	@Type   int,
	@StatusType int,
	@Date datetime
)
AS
BEGIN    
	IF(@StatusType = 4)
	     Update ReportType SET Amz_ReportId=@Amz_ReportId,RefreshStatus=@Status,InventoryAttempt=0,ApiAttempt=0,ReportReqDate=@Date,
		 ModifiedOn=@Date,RefreshedOn=GETDATE()
         WHERE ReportId=@ReportId and RecordType=@Type
	ELSE
        Update ReportType SET Amz_ReportId=@Amz_ReportId,ReportStatus=@Status,InventoryAttempt=0,ApiAttempt=0,ReportReqDate=@Date,ModifiedOn=@Date
                             WHERE ReportId=@ReportId and RecordType=@Type
END
