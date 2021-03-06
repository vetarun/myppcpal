IF EXISTS(
        SELECT*
        FROM sys.objects
        WHERE object_id = OBJECT_ID(N'[dbo].[Sbsp_GetUserReportData]')

            AND type IN (
                N'P'
				, N'PC'
				) 
		)
	DROP PROCEDURE[dbo].[Sbsp_GetUserReportData]
GO 
CREATE PROCEDURE [dbo].[Sbsp_GetUserReportData]
	@userId int,
	@date date,
	@type int
AS
BEGIN
	SET NOCOUNT ON;
IF(@type=1)
 BEGIN
	SELECT c.Name as Campaign_Name, CASE WHEN c.DailyBudget=0 THEN '0' ELSE '$'+CAST(CAST(c.DailyBudget as decimal(10,2)) as varchar(20)) END as  Campaign_Daily_Budget,
	c.TargetingType Campaign_Targeting_Type, a.Name Ad_Group_Name, CASE WHEN k.Bid =0 THEN '0' ELSE '$'+CAST(CAST(k.Bid as decimal(10,2)) as varchar(20)) END Max_Bid, k.Keyword,
	k.MatchType Match_Type, c.Status Campaign_Status, k.Status Keyword_Status, a.Status Ad_Group_Status,SUM(i.Impressions) as Impressions,
	SUM(i.Clicks) as Clicks,CASE WHEN SUM(i.Spend)=0 THEN '0' ELSE '$'+CAST(CAST(SUM(i.Spend)as decimal(10,2)) as varchar(20)) END as Spend,
	SUM(i.Orders) as Orders, CASE WHEN SUM(i.Sales)=0 THEN '0' ELSE '$'+CAST(CAST(SUM(i.Sales)as decimal(10,2)) as varchar(20)) END as Sales,
	CASE WHEN SUM(i.Sales)>0 THEN CAST(CAST(ROUND(((SUM(i.Spend) / SUM(i.Sales)) * 100),2,1)as decimal(10,2)) as varchar(20))+'%' ELSE '0' END AS ACoS from Reports r 
	INNER JOIN ProductsInventory i on r.ReportID = i.ReportID 
	INNER JOIN Keywords k on k.RecordID = i.RecordID INNER JOIN Campaigns c on c.RecordID = k.CampaignID
	INNER JOIN AdGroups a on a.RecordID = k.AdGroupID
	WHERE i.RecordType = 3 and ReportDate BETWEEN  DATEADD(dd,-62,@date) AND DATEADD(dd,-3,@date) and r.MppUserID=@userId and k.Status = 'enabled' 
	GROUP BY c.Name, c.DailyBudget,c.TargetingType,a.Name,k.Bid,k.Keyword,k.MatchType,c.Status,k.Status,a.Status ORDER BY k.Keyword asc

	SELECT k.Keyword,SUM(i.Impressions) as Impressions,SUM(i.Clicks) as Clicks,CASE WHEN SUM(i.Spend)=0 THEN '0' ELSE '$'+CAST(CAST(SUM(i.Spend)as decimal(10,2)) as varchar(20)) END as Spend,
	SUM(i.Orders) as Orders,CASE WHEN SUM(i.Sales)=0 THEN '0' ELSE '$'+CAST(CAST(SUM(i.Sales)as decimal(10,2)) as varchar(20)) END as Sales,
	CASE WHEN SUM(i.Sales)>0 THEN CAST(CAST(ROUND(((SUM(i.Spend) / SUM(i.Sales)) * 100),2,1)as decimal(10,2)) as varchar(20))+'%'
	ELSE '0' END AS ACoS from Reports r 
	INNER JOIN ProductsInventory i on r.ReportID = i.ReportID 
	INNER JOIN Keywords k on k.RecordID = i.RecordID INNER JOIN Campaigns c on c.RecordID = k.CampaignID
	INNER JOIN AdGroups a on a.RecordID = k.AdGroupID
	WHERE i.RecordType = 3 and ReportDate BETWEEN  DATEADD(dd,-62,@date) AND DATEADD(dd,-3,@date) and r.MppUserID=@userId and k.Status = 'enabled' 
	GROUP BY k.Keyword ORDER BY k.Keyword asc
END
ELSE IF(@type=3)
 BEGIN
    SELECT c.Name as Campaign_Name,CASE WHEN c.DailyBudget=0 THEN '0' ELSE '$'+CAST(CAST(c.DailyBudget as decimal(10,2)) as varchar(20)) END as  Campaign_Daily_Budget,
	c.TargetingType Campaign_Targeting_Type,a.Name Ad_Group_Name, CASE WHEN k.Bid =0 THEN '0' ELSE '$'+CAST(CAST(k.Bid as decimal(10,2)) as varchar(20)) END Max_Bid, k.Keyword,
	k.MatchType Match_Type, st.Query Search_Term, c.Status Campaign_Status,k.Status Keyword_Status, a.Status Ad_Group_Status,SUM(st.Impressions) as Impressions,
	SUM(st.Clicks) as Clicks,CASE WHEN SUM(st.Spend)=0 THEN '0' ELSE '$'+CAST(CAST(SUM(st.Spend)as decimal(10,2)) as varchar(20)) END as Spend,
	SUM(st.Orders) as Orders,CASE WHEN SUM(st.Sales)=0 THEN '0' ELSE '$'+CAST(CAST(SUM(st.Sales)as decimal(10,2)) as varchar(20)) END as Sales,
	CASE WHEN SUM(st.Sales)>0 THEN CAST(CAST(ROUND(((SUM(st.Spend) / SUM(st.Sales)) * 100),2,1)as decimal(10,2)) as varchar(20))+'%'
	ELSE '0' END AS ACoS from Reports r 
	INNER JOIN SearchtermInventory st on r.ReportID = st.ReportID 
	INNER JOIN Keywords k on k.RecordID = st.RecordID 
	INNER JOIN Campaigns c on c.RecordID = k.CampaignID and st.RecordID=k.RecordID
	INNER JOIN AdGroups a on a.RecordID = k.AdGroupID
	WHERE  ReportDate BETWEEN DATEADD(dd,-62,@date) AND DATEADD(dd,-3,@date) and r.MppUserID=@userId and k.Status = 'enabled' 
	GROUP BY c.Name,c.DailyBudget,c.TargetingType,a.Name,k.Bid,k.Keyword,k.MatchType,c.Status,k.Status,a.Status,st.Query ORDER BY k.Keyword asc

    SELECT k.Keyword,st.Query Search_Term, SUM(st.Impressions) as Impressions,SUM(st.Clicks) as Clicks,CASE WHEN SUM(st.Spend)=0 THEN '0' ELSE 
    '$'+CAST(CAST(SUM(st.Spend)as decimal(10,2)) as varchar(20)) END as Spend, SUM(st.Orders) as Orders,CASE WHEN SUM(st.Sales)=0 THEN '0' ELSE
	'$'+CAST(CAST(SUM(st.Sales)as decimal(10,2)) as varchar(20)) END as Sales,CASE WHEN SUM(st.Sales)>0 THEN
	CAST(CAST(ROUND(((SUM(st.Spend) / SUM(st.Sales)) * 100),2,1)as decimal(10,2)) as varchar(20))+'%' ELSE '0' END AS ACoS from Reports r 
	INNER JOIN SearchtermInventory st on r.ReportID = st.ReportID 
	INNER JOIN Keywords k on k.RecordID = st.RecordID 
	INNER JOIN Campaigns c on c.RecordID = k.CampaignID and st.RecordID=k.RecordID
	INNER JOIN AdGroups a on a.RecordID = k.AdGroupID
	WHERE  ReportDate BETWEEN DATEADD(dd,-62,@date) AND DATEADD(dd,-3,@date) and r.MppUserID=@userId and k.Status = 'enabled' 
	GROUP BY k.Keyword,st.Query ORDER BY k.Keyword asc
 END

 ELSE IF(@type=5)
 BEGIN
    SELECT c.Name as Campaign_Name, CASE WHEN c.DailyBudget=0 THEN '0' ELSE '$'+CAST(CAST(c.DailyBudget as decimal(10,2)) as varchar(20)) END as  Campaign_Daily_Budget,
	c.TargetingType Campaign_Targeting_Type, a.Name Ad_Group_Name, CASE WHEN k.Bid =0 THEN '0' ELSE '$'+CAST(CAST(k.Bid as decimal(10,2)) as varchar(20)) END Max_Bid,
	k.Keyword, k.MatchType Match_Type, c.Status Campaign_Status,k.Status Keyword_Status, a.Status Ad_Group_Status,SUM(i.Impressions) as Impressions,
	SUM(i.Clicks) as Clicks, CASE WHEN SUM(i.Spend)=0 THEN '0' ELSE '$'+CAST(CAST(SUM(i.Spend)as decimal(10,2)) as varchar(20)) END as Spend,
	SUM(i.Orders) as Orders, CASE WHEN SUM(i.Sales)=0 THEN '0' ELSE '$'+CAST(CAST(SUM(i.Sales)as decimal(10,2)) as varchar(20)) END as Sales,
	CASE WHEN SUM(i.Sales)>0 THEN CAST(CAST(ROUND(((SUM(i.Spend) / SUM(i.Sales)) * 100),2,1)as decimal(10,2)) as varchar(20))+'%' ELSE '0' END AS ACoS from Reports r
	INNER JOIN ProductsInventory i  ON r.ReportID=i.ReportID
	INNER JOIN OptimizeKeyLog opt ON opt.KeywordID=i.RecordID and opt.UpdateStatus=1
	INNER JOIN Keywords k on k.RecordID = i.RecordID 
	INNER JOIN Campaigns c on c.RecordID = k.CampaignID
	INNER JOIN AdGroups a on a.RecordID = k.AdGroupID
	WHERE i.RecordType = 3 and (r.ReportDate BETWEEN DATEADD(dd,-62,@date) AND DATEADD(dd,-3,@date)) and CAST(opt.ModifiedOn as date)=CAST(@date as date) and i.MppUserID=@userId 
	GROUP BY c.Name,c.DailyBudget,c.TargetingType,a.Name,k.Bid,k.Keyword,k.MatchType,c.Status,k.Status,a.Status ORDER BY k.Keyword asc
	   
	SELECT k.Keyword,SUM(i.Impressions) as Impressions,SUM(i.Clicks) as Clicks, CASE WHEN SUM(i.Spend)=0 THEN '0' ELSE 
	'$'+CAST(CAST(SUM(i.Spend)as decimal(10,2)) as varchar(20)) END as Spend, SUM(i.Orders) as Orders, CASE WHEN SUM(i.Sales)=0 THEN '0'
	ELSE '$'+CAST(CAST(SUM(i.Sales)as decimal(10,2)) as varchar(20)) END as Sales, CASE WHEN SUM(i.Sales)>0 THEN
	CAST(CAST(ROUND(((SUM(i.Spend) / SUM(i.Sales)) * 100),2,1)as decimal(10,2)) as varchar(20))+'%' ELSE '0' END AS ACoS  from Reports r
	INNER JOIN ProductsInventory i  ON r.ReportID=i.ReportID 
	INNER JOIN OptimizeKeyLog opt ON opt.KeywordID=i.RecordID and opt.UpdateStatus=1
	INNER JOIN Keywords k on k.RecordID = i.RecordID 
	INNER JOIN Campaigns c on c.RecordID = k.CampaignID
	INNER JOIN AdGroups a on a.RecordID = k.AdGroupID
	WHERE i.RecordType = 3 and (r.ReportDate BETWEEN DATEADD(dd,-62,@date) AND DATEADD(dd,-3,@date)) and CAST(opt.ModifiedOn as date)=CAST(@date as date) and i.MppUserID=@userId 
	GROUP BY k.Keyword ORDER BY k.Keyword asc
 END
 ELSE IF(@type=6)
 BEGIN
	SELECT c.Name as Campaign_Name,CASE WHEN c.DailyBudget=0 THEN '0' ELSE '$'+CAST(CAST(c.DailyBudget as decimal(10,2)) as varchar(20)) END as  Campaign_Daily_Budget,
	c.TargetingType Campaign_Targeting_Type,a.Name Ad_Group_Name,CASE WHEN k.Bid =0 THEN '0' ELSE '$'+CAST(CAST(k.Bid as decimal(10,2)) as varchar(20)) END Max_Bid,
	k.Keyword,k.MatchType Match_Type,i.Query Search_Term,c.Status Campaign_Status,k.Status Keyword_Status, a.Status Ad_Group_Status,
	SUM(i.Impressions) as Impressions,SUM(i.Clicks) as Clicks,CASE WHEN SUM(i.Spend)=0 THEN '0' ELSE '$'+CAST(CAST(SUM(i.Spend)as decimal(10,2)) as varchar(20)) END as Spend,
	SUM(i.Orders) as Orders,CASE WHEN SUM(i.Sales)=0 THEN '0' ELSE '$'+CAST(CAST(SUM(i.Sales)as decimal(10,2)) as varchar(20)) END as Sales,
	CASE WHEN SUM(i.Sales)>0 THEN CAST(CAST(ROUND(((SUM(i.Spend) / SUM(i.Sales)) * 100),2,1)as decimal(10,2)) as varchar(20))+'%'
	ELSE '0' END AS ACoS from Reports r
	INNER JOIN SearchtermInventory i  ON r.ReportID=i.ReportID
	INNER JOIN OptimizeKeyLog opt ON opt.KeywordID=i.RecordID and opt.UpdateStatus=1
	INNER JOIN Keywords k on k.RecordID = i.RecordID 
	INNER JOIN Campaigns c on c.RecordID = k.CampaignID
	INNER JOIN AdGroups a on a.RecordID = k.AdGroupID
	WHERE (ReportDate BETWEEN DATEADD(dd,-62,@date) AND DATEADD(dd,-3,@date)) and CAST(opt.ModifiedOn as date)=CAST(@date as date) and i.MppUserID=@userId 
	GROUP BY c.Name,c.DailyBudget,c.TargetingType,a.Name,k.Bid,k.Keyword,k.MatchType,c.Status,k.Status,a.Status,i.Query ORDER BY k.Keyword asc

	SELECT k.Keyword, i.Query Search_Term,SUM(i.Impressions) as Impressions,SUM(i.Clicks) as Clicks, CASE WHEN SUM(i.Spend)=0 THEN '0'
	ELSE '$'+CAST(CAST(SUM(i.Spend)as decimal(10,2)) as varchar(20)) END as Spend, SUM(i.Orders) as Orders, CASE WHEN SUM(i.Sales)=0 THEN '0'
	ELSE '$'+CAST(CAST(SUM(i.Sales)as decimal(10,2)) as varchar(20)) END as Sales, CASE WHEN SUM(i.Sales)>0 THEN
	CAST(CAST(ROUND(((SUM(i.Spend) / SUM(i.Sales)) * 100),2,1)as decimal(10,2)) as varchar(20))+'%' ELSE '0' END AS ACoS from Reports r
	INNER JOIN SearchtermInventory i  ON r.ReportID=i.ReportID  
	INNER JOIN OptimizeKeyLog opt ON opt.KeywordID=i.RecordID and opt.UpdateStatus=1
	INNER JOIN Keywords k on k.RecordID = i.RecordID 
	INNER JOIN Campaigns c on c.RecordID = k.CampaignID
	INNER JOIN AdGroups a on a.RecordID = k.AdGroupID
	WHERE (ReportDate BETWEEN DATEADD(dd,-62,@date) AND DATEADD(dd,-3,@date)) and CAST(opt.ModifiedOn as date)=CAST(@date as date) and i.MppUserID=@userId 
	GROUP BY k.Keyword,i.Query ORDER BY k.Keyword asc
 END
END

   
                       
                                       

