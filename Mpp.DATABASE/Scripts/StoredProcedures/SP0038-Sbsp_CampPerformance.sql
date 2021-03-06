IF EXISTS(
        SELECT*
        FROM sys.objects
        WHERE object_id = OBJECT_ID(N'[dbo].[Sbsp_CampPerformance]')

            AND type IN (
                N'P'
				, N'PC'
				) 
		)
	DROP PROCEDURE[dbo].[Sbsp_CampPerformance]
GO 

CREATE procedure [dbo].[Sbsp_CampPerformance]
( @campID int ,
@keyName varchar ,
@order int ,

@type int,
@UserID int )
AS 
BEGIN
IF(@type=1)
BEGIN
SELECT Keyword
      ,lo.ModifiedField
      ,lo.OldValue
      ,lo.NewValue
      ,lo.ReasonID
      ,lo.UpdateStatus
   Keyword , ky.date FROM keywords k inner join ( SELECT  KeywordID, max(createdon) AS date  FROM optimizekeylog  
    GROUP BY KeywordID) ky ON k.RecordID = ky.KeywordID
	join OptimizeKeyLog AS lo ON ky.KeywordID=lo.KeywordID
   join Campaigns AS c ON lo.CampaignID=c.RecordID WHERE lo.CampaignID = @campId
   and  c.MppUserID = @UserID
   END 
   ELSE IF (@type=0)
   --//campaign optimization  
 BEGIN
 IF(@order=1)
 BEGIN
  SELECT c.Name as campaign, KY.CampaignID AS CiD,
    ky.date from  Campaigns as c inner join ( select  CampaignID, max(createdon) as date  from optimizekeylog  
    group by CampaignID) ky on c.RecordID = ky.CampaignID
	WHERE c.MppUserID = @UserID
	ORDER BY  date asc 
 END
 ELSE
 BEGIN 
 SELECT c.Name as campaign, KY.CampaignID AS CiD,
    ky.date from  Campaigns as c inner join ( select  CampaignID, max(createdon) as date  from optimizekeylog  
    group by CampaignID) ky on c.RecordID = ky.CampaignID
	where c.MppUserID = @UserID
	order by  date desc 
 END
 END
  ELSE
  BEGIN
  SELECT Keyword
      ,kl.ModifiedField
      ,kl.OldValue
      ,kl.NewValue
      ,kl.ReasonID
      ,kl.UpdateStatus
   Keyword , kl.CreatedOn from keywords k inner join 
  OptimizeKeyLog as kl on k.RecordID=kl.KeywordID where Keyword= @KeyName
  END 
 END 
