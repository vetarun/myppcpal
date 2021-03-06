IF EXISTS(
        SELECT*
        FROM sys.objects
        WHERE object_id = OBJECT_ID(N'[dbo].[Sbsp_GetReportLog]')

            AND type IN (
                N'P'
				, N'PC'
				) 
		)
	DROP PROCEDURE[dbo].[Sbsp_GetReportLog]
GO 
CREATE PROCEDURE [dbo].[Sbsp_GetReportLog]      
(  
@orderBy VARCHAR(100),  
@dir BIT,  
@pageNumber INT,  
@pageSize INT,  
@client INT,  
@date DATE,  
@date2 DATE,  
@total INT OUTPUT  
)  
AS  
BEGIN  
 SET NOCOUNT ON;  
 DECLARE @SQLStatement varchar(MAX)  
 DECLARE @direction varchar(5)  
 DECLARE @skip int  
  
 SET @skip=(@pageNumber-1)*@pageSize  
 SELECT @total=COUNT(*) FROM Reports rp  
 JOIN ReportType rt ON  rt.ReportId=rp.ReportId  
   
 WHERE  rt.ReportReqDate IS NOT NULL AND (rp.MppUserID = CASE   
                   WHEN @client = 0 THEN rp.MppUserID    
                   ELSE @client  
                 END )   
       AND   rt.ReportReqDate BETWEEN @date and @date2 
SELECT   
    usr.FirstName+' '+usr.LastName Name,  
 rp.ReportDate,  
 ISNULL(rp.UpdtNegStatus,0) UpdtNegStatus,  
 rct.RecordName,  
 ISNULL(rt.RefStatusCount,0) RefStatusCount,  
 ISNULL(rp.UpdtBidStatus,0) UpdtBidStatus,  
 rp.OptimizeDate,  
 ISNULL(rt.ReportStatus,0) ReportStatus ,  
 rt.ReportReqDate,  
 ISNULL(rt.RefreshStatus,0) RefreshStatus  
    FROM Reports rp  
 JOIN ReportType rt ON rt.ReportId=rp.ReportID  
 JOIN RecordType rct ON rct.RecordTypeId=rt.RecordType  
    JOIN MppUser usr ON usr.MppUserID=rp.MppUserID   
 WHERE  rt.ReportReqDate IS NOT NULL AND  
        rp.MppUserID  = CASE   
                   WHEN @client = 0 THEN rp.MppUserID   
                    ELSE @client  
                    END 
       AND rt.ReportReqDate >= @date and rt.ReportReqDate< @date2 
 ORDER BY 
                    CASE WHEN @orderby = 'Name' AND @dir = 1 THEN usr.FirstName END DESC,    
                    CASE WHEN @orderby = 'Name' AND @dir = 0 THEN usr.FirstName END ASC, 
					   
                    CASE WHEN @orderby = 'RecordName' AND @dir = 1 THEN rct.RecordName END DESC,
                    CASE WHEN @orderby = 'RecordName' AND @dir = 0 THEN rct.RecordName END ASC,

					CASE WHEN @orderby = 'ReportDate' AND @dir = 1 THEN rp.ReportDate END DESC,
                    CASE WHEN @orderby = 'ReportDate' AND @dir = 0 THEN rp.ReportDate END ASC,

                    CASE WHEN @orderby = 'OptimizeDate' AND @dir = 1 THEN rp.OptimizeDate  END DESC,
                    CASE WHEN @orderby = 'OptimizeDate' AND @dir = 0 THEN  rp.OptimizeDate  END ASC,

			        CASE WHEN @orderby = 'RefStatusCount' AND @dir = 1 THEN rt.RefStatusCount END DESC,
                    CASE WHEN @orderby = 'RefStatusCount' AND @dir = 0 THEN rt.RefStatusCount END ASC,

					CASE WHEN @orderby = 'ReportStatus' AND @dir = 1 THEN rt.ReportStatus END DESC,    
                    CASE WHEN @orderby = 'ReportStatus' AND @dir = 0 THEN rt.ReportStatus END ASC,
					    
                    CASE WHEN @orderby = 'RefreshStatus' AND @dir = 1 THEN rt.RefreshStatus END DESC,
                    CASE WHEN @orderby = 'RefreshStatus' AND @dir = 0 THEN rt.RefreshStatus END ASC,

					CASE WHEN @orderby = 'UpdtBidStatus' AND @dir = 1 THEN rp.UpdtBidStatus END DESC,
                    CASE WHEN @orderby = 'UpdtBidStatus' AND @dir = 0 THEN rp.UpdtBidStatus END ASC,

                    CASE WHEN @orderby = 'UpdtNegStatus' AND @dir = 1 THEN rp.UpdtNegStatus END DESC,
                    CASE WHEN @orderby = 'UpdtNegStatus' AND @dir = 0 THEN rp.UpdtNegStatus END ASC
 
                      OFFSET @skip  ROWS FETCH NEXT @pageSize ROWS ONLY;  
END  
