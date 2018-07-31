IF EXISTS(
        SELECT*
        FROM sys.objects
        WHERE object_id = OBJECT_ID(N'[dbo].[Sbsp_DeleteArchiveData]')

            AND type IN (
                N'P'
				, N'PC'
				)
		)
	DROP PROCEDURE[dbo].[Sbsp_DeleteArchiveData]
GO 
CREATE  procedure  Sbsp_DeleteArchiveData
( @ArchiveDate   smalldatetime 
)
AS
Begin 
BEGIN TRANSACTION Del
DECLARE @userId int
	DECLARE @DelUserID CURSOR 
    SET @DelUserID = CURSOR STATIC FOR SELECT MppUserID from MppUser  where dateAdd(day,2,IsArchiveOn) < @ArchiveDate
    OPEN @DelUserID
    FETCH NEXT FROM @DelUserID INTO @userId
    WHILE @@FETCH_STATUS = 0
    BEGIN    

DELETE  ad FROM AdGroupThirtyDay as ad join MppReports as r on ad.ReportID = r.ReportID  WHERE r.MppUserID =@userId
DELETE  c FROM CampaignOneDay as c join MppReports as r on c.ReportID = r.ReportID  WHERE r.MppUserID =@userId
DELETE  k FROM KeywordOneDay as k join MppReports as r on k.ReportID = r.ReportID  WHERE r.MppUserID =@userId
DELETE  ad FROM AdGroupOneDay as ad join MppReports as r on ad.ReportID = r.ReportID  WHERE r.MppUserID =@userId
DELETE  ad FROM AdGroupSixtyDay as ad join MppReports as r on ad.ReportID = r.ReportID  WHERE r.MppUserID =@userId
DELETE  k FROM KeywordNegative as k join MppReports as r on k.ReportID = r.ReportID  WHERE r.MppUserID =@userId
DELETE  k FROM KeywordSixtyDay as k join MppReports as r on k.ReportID = r.ReportID  WHERE r.MppUserID =@userId
DELETE  k FROM KeywordThirtyDay as k join MppReports as r on k.ReportID = r.ReportID  WHERE r.MppUserID =@userId
DELETE  dc FROM DailyCount as dc join MppReports as r on dc.ReportID = r.ReportID  WHERE r.MppUserID =@userId
DELETE  c FROM CampaignThirtyDay as c join MppReports as r on c.ReportID = r.ReportID  WHERE r.MppUserID =@userId
DELETE  c FROM CampaignSixtyDay as c join MppReports as r on c.ReportID = r.ReportID  WHERE r.MppUserID =@userId
DELETE FROM MppReports where MppUserID=@userId
DELETE  rt FROM ReportType as rt join Reports as r on rt.ReportID = r.ReportID  WHERE r.MppUserID =@userId
DELETE  p FROM ProductsInventory as p join Reports as r on p.ReportID = r.ReportID  WHERE r.MppUserID =@userId
DELETE  s FROM SearchtermInventory as s join Reports as r on s.ReportID = r.ReportID  WHERE r.MppUserID =@userId

DELETE from Reports WHERE MppUserID=@userId
delete from mppuser where MppUserID=@userId
FETCH NEXT FROM @DelUserID INTO @userId
    END
	CLOSE @DelUserID
	DEALLOCATE @DelUserID

commit
End