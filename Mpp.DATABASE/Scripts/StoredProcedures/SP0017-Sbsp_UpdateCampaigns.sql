IF EXISTS(
        SELECT*
        FROM sys.objects
        WHERE object_id = OBJECT_ID(N'[dbo].[Sbsp_UpdateCampaigns]')

            AND type IN (
                N'P'
				, N'PC'
				) 
		)
	DROP PROCEDURE[dbo].[Sbsp_UpdateCampaigns]
	GO
CREATE PROCEDURE [dbo].[Sbsp_UpdateCampaigns]
(	
	@tblCampaigns sp_CampaignType READONLY,
	@UserID  int,
	@ReportId int,
	@RecordType int,
	@Email varchar(100) out,
	@Date smalldatetime
)  
AS
BEGIN
BEGIN TRANSACTION
SET XACT_ABORT ON
      SET @Email = ''
      ALTER TABLE Campaigns DISABLE TRIGGER Sbtr_Campaigns_Update
      MERGE INTO Campaigns c1
      USING @tblCampaigns c2
      ON c1.RecordID=c2.RecordID and c1.MppUserID=@UserID
      WHEN MATCHED THEN
      UPDATE SET c1.Name = c2.Name, c1.DailyBudget = c2.DailyBudget,c1.StartDate=c2.StartDate,c1.EndDate=c2.EndDate,
			c1.TargetingType=c2.TargetingType,c1.Status=c2.Status,c1.ModifiedOn=GetDate()
      WHEN NOT MATCHED THEN 
      INSERT VALUES(c2.RecordID,@UserID,c2.Name,2,0,c2.DailyBudget,c2.StartDate,c2.EndDate,c2.TargetingType,c2.Status,null,getdate(),null,null, 0, 0);

	
	  ALTER TABLE Campaigns ENABLE TRIGGER Sbtr_Campaigns_Update
	  
	  Exec Sbsp_CreateFormulaForCamp @UserID
	 
	        Update ReportType set SnapStatus = 2, ModifiedOn=GetDate() where ReportId = @ReportId and RecordType=@RecordType
			IF EXISTS(SELECT 1 from Campaigns where StartDate=@Date and MppUserID = @UserID)
	        BEGIN
	         SET @Email = (SELECT Email from MppUser where MppUserID = @UserID and StartDate < DATEADD(d,3,@Date))
	        END
	
	  SELECT @Email Email
COMMIT
END
