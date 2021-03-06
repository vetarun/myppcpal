
IF EXISTS(
        SELECT*
        FROM sys.objects
        WHERE object_id = OBJECT_ID(N'[dbo].[Sbsp_GetUserReportDates]')

            AND type IN (
                N'P'
				, N'PC'
				)
		)
	DROP PROCEDURE[dbo].[Sbsp_GetUserReportDates]
	GO
CREATE PROCEDURE [dbo].[Sbsp_GetUserReportDates] 
@userId int 
AS
BEGIN
	SET NOCOUNT ON;
   SELECT TOP 30 DATEADD(DAY,3,CAST(r.ReportDate as DATE)) KeyBulkDates FROM Reports r 
  JOIN ReportType rt ON r.ReportId=rt.ReportId AND rt.ReportStatus=2 AND rt.RecordType=1
  JOIN ProductsInventory i ON i.ReportID =rt.ReportId
  WHERE r.MppUserID=@userId AND CAST(r.ReportDate as DATE) BETWEEN DATEADD(DAY,-33,CAST(GETDATE() as DATE)) AND DATEADD(DAY,1,CAST(GETDATE() as DATE)) 
  GROUP BY  DATEADD(DAY,3,CAST(r.ReportDate as DATE))
  ORDER BY KeyBulkDates DESC
  

   SELECT TOP 30 DATEADD(DAY,3,CAST(r.ReportDate as DATE)) StermBulkDates FROM Reports r 
  JOIN ReportType rt ON r.ReportId=rt.ReportId AND rt.ReportStatus=2 AND rt.RecordType=1
  JOIN SearchtermInventory i ON i.ReportID =rt.ReportId
  WHERE r.MppUserID=@userId AND CAST(r.ReportDate as DATE) BETWEEN DATEADD(DAY,-33,CAST(GETDATE() as DATE)) AND DATEADD(DAY,1,CAST(GETDATE() as DATE)) 
  GROUP BY  DATEADD(DAY,3,CAST(r.ReportDate as DATE))
  ORDER BY StermBulkDates DESC

   
 SELECT TOP 30 CAST(o.CreatedOn as date) as KeyUpload FROM Keywords k 
 INNER JOIN OptimizeKeyLog o on k.RecordID=o.KeywordID
 INNER JOIN ProductsInventory p ON p.RecordID=o.KeywordID
 where k.MppUserID=@userId  AND o.CreatedOn BETWEEN DATEADD(dd,-33,GETDATE()) AND GETDATE() AND o.UpdateStatus=1
 GROUP BY  CAST(o.CreatedOn as date)
 ORDER BY KeyUpload DESC  

 SELECT TOP 30 CAST(o.CreatedOn as date) as StermUpload FROM Keywords k 
 INNER JOIN OptimizeKeyLog o on k.RecordID=o.KeywordID
 INNER JOIN SearchtermInventory p ON p.RecordID=o.KeywordID
 where k.MppUserID=@userId  and o.CreatedOn BETWEEN DATEADD(dd,-33,GETDATE()) AND GETDATE() AND o.UpdateStatus=1
 GROUP BY  CAST(o.CreatedOn as date)  
 ORDER BY StermUpload desc  

END
