
IF EXISTS(
        SELECT*
        FROM sys.objects
        WHERE object_id = OBJECT_ID(N'[dbo].[Sbsp_GetOptimizeKeywordsAndFormula]')

            AND type IN (
                N'P'
				, N'PC'
				) 
		)
	DROP PROCEDURE[dbo].[Sbsp_GetOptimizeKeywordsAndFormula]
GO 
CREATE PROCEDURE [dbo].[Sbsp_GetOptimizeKeywordsAndFormula]
(	
	@UserID  int,
	@enddate smalldatetime  
)  
AS
BEGIN
     DECLARE @startdate smalldatetime
	 SET @startdate = DATEADD(d, -59,@enddate)
	 DECLARE @Count int = 0
	 
	 SET @Count = (SELECT count(*) from ReportType where ReportId IN (SELECT ReportId from Reports where MppUserID=@UserID and ReportDate BETWEEN @StartDate and @EndDate) and ReportStatus=2 and RecordType= 1 )
	 
	 IF(@Count > 56)
	 BEGIN
		 SELECT c.RecordID as CampaignID,f.AcosPause,f.AcosLower,f.AcosRaise,f.SpendPause,f.SpendLower,f.ClicksPause,f.ClicksLower,f.CTRPause,
		 f.CTRLower,f.BidRaise,f.BidLower,f.MinBid,f.MaxBid from Campaigns as c JOIN formula as f on c.FormulaID=f.FormulaID where c.MppUserID=@UserID and c.Active =1

		 SELECT k.CampaignID as CampaignID,k.AdgroupID,k.RecordID as KeywordID,k.Status,k.Bid, sum(p.Sales) as Sales,sum(p.Orders) as Orders,sum(p.Spend) as Spend,sum(p.Clicks) as Clicks,sum(p.Impressions) as Impressions
		 FROM Keywords k join ProductsInventory as p on k.RecordID = p.RecordID JOIN Reports r on p.ReportID=r.ReportID where k.MppUserID=@UserID and r.ReportDate BETWEEN @startdate and @enddate 
		 and k.CampaignID IN(SELECT RecordID from Campaigns c where c.Active = 1)
		 and DATEADD(DAY,30, ISNULL(k.ManuallyChangedOn, DATEADD(DAY, -31, GETDATE())))<GETDATE()
		  group by k.CampaignID,k.AdgroupID,k.RecordID,k.Status,k.Bid
	 END
END
