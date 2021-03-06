IF EXISTS(
        SELECT*
        FROM sys.objects
        WHERE object_id = OBJECT_ID(N'[dbo].[Sbsp_GetOptimizeKeyLog]')

            AND type IN (
                N'P'
				, N'PC'
				) 
		)
	DROP PROCEDURE[dbo].[Sbsp_GetOptimizeKeyLog]
GO 
CREATE PROCEDURE [dbo].[Sbsp_GetOptimizeKeyLog]
(
	@CampId	            bigint,
	@UserID             int
)
AS
BEGIN
DECLARE @CampName varchar(200)
declare @PopUp int 
SET @CampName = ISNULL((SELECT Name FROM Campaigns WHERE RecordID = @CampId), '')
set @PopUp = (SELECT max(PopUpID) from PopUpAlert where MppUserID = @UserID)
if (@PopUp is null)
begin
set @PopUp = 0 
end
IF(@CampName <> '')
BEGIN
SELECT @PopUp as PopUpID,KeyName,AdGroupName, ModifiedField, ISNULL(OldValue, '-') as OldValue, NewValue, Reasondesc, ol.CreatedOn as ModifiedOn
                                  FROM OptimizeLog ol, OptimizeReason opr
                                  WHERE ol.ReasonID=opr.ReasonID and ol.MppUserID=@UserID and ol.CampName= @CampName
END

SELECT @PopUp as PopUpID ,a.Name as AdGroupName,k.Keyword as KeyName ,o.KeywordID,o.ModifiedField, O.ReportID, o.ReasonID,o.CampaignID,o.AdgroupID, ISNULL(o.OldValue, '-') as OldValue, o.NewValue, Reasondesc, o.CreatedOn as ModifiedOn
,case when (dateadd(day,30, o.ManuallyChangedOn )>getdate())  then 1 else 0 end as RevertStatus ,
case when (lower(k.MatchType) = 'broad') then 'B' when (lower(k.MatchType) = 'Exact') then 'E'else 'P' end as MatchType from OptimizeKeyLog o
INNER JOIN AdGroups a on o.AdGroupID = a.RecordID
INNER JOIN Keywords k on o.KeywordID = k.RecordID
INNER JOIN OptimizeReason r on o.ReasonID=r.ReasonID and o.Type = 3 and o.CampaignID= @CampId and o.ReportID IN (SELECT ReportID from Reports where MppUserID=@UserID)

SELECT  @PopUp as PopUpID,a.Name as AdGroupName,k.Keyword as KeyName,o.ModifiedField,o.ReportID, o.ReasonID,o.KeywordID,o.CampaignID,o.AdgroupID, ISNULL(o.OldValue, '-') as OldValue, o.NewValue, Reasondesc, o.CreatedOn as ModifiedOn
, case when (dateadd(day,30, o.ManuallyChangedOn )>getdate())  then 1 else 0 end as RevertStatus,
case when (lower(k.MatchType) = 'broad') then 'B' when (lower(k.MatchType) = 'Exact') then 'E'else 'P' end as MatchType
 from OptimizeKeyLog o
INNER JOIN AdGroups a on o.AdGroupID = a.RecordID

INNER JOIN Keywords k on o.KeywordID = k.RecordID

INNER JOIN OptimizeReason r on o.ReasonID=r.ReasonID and o.Type = 4 and o.CampaignID= @CampId and o.ReportID IN (SELECT ReportID from Reports where MppUserID=@UserID)



END