IF EXISTS(
        SELECT*
        FROM sys.objects
        WHERE object_id = OBJECT_ID(N'[dbo].[Sbsp_GetFormulaForCamp]')

            AND type IN (
                N'P'
				, N'PC'
				) 
		)
	DROP PROCEDURE[dbo].[Sbsp_GetFormulaForCamp]
GO 
CREATE PROCEDURE [dbo].[Sbsp_GetFormulaForCamp] 
( 

	@UserID int, 
	@RecordID bigint
)
AS
BEGIN
DECLARE @FormulaName varchar(20)
DECLARE @Id int
DECLARE @val decimal
SET @val = 0
SELECT @FormulaName = FormulaName,@Id = FormulaID  FROM Formula where FormulaID in (select FormulaID from Campaigns where RecordID=@RecordID and MppUserID=@UserID)
IF(@FormulaName = 'Conservative' or @FormulaName = 'Recommended' or @FormulaName = 'Aggressive')
BEGIN
Select @Id as FormulaID, @FormulaName as FormulaName, @val as AcosPause,@val as AcosLower,@val as AcosRaise,@val as AcosNegative,
	   @val as SpendPause,@val as SpendLower,@val as SpendNegative,@val as ClicksPause
       ,@val as ClicksLower,@val as ClicksNegative,@val as CTRPause,@val as CTRLower,
	   @val as CTRNegative,@val as BidRaise,@val as BidLower, MinBid,TargetingType, MaxBid ,
	   ISNULL(bst.ACoSCutOff,0) BestACoSCutOff,CAST(ISNULL(bst.ImpressionCutOff,0) AS decimal) BestImpressionCutOff,
	   ISNULL(bst.ID,0) BSTId
	   from Formula as f join Campaigns as c on f.FormulaID=c.FormulaID 
	   LEFT JOIN BestSearchTermRequest bst ON bst.CampaignId=c.RecordID AND bst.MppUserId=@UserID
	   where f.FormulaID = @Id
END
ELSE
BEGIN
Select @Id as FormulaID,@FormulaName as FormulaName, AcosPause,AcosLower,AcosRaise,AcosNegative,
	   SpendPause,SpendLower,SpendNegative,ClicksPause, TargetingType
       ,ClicksLower,ClicksNegative,CTRPause,CTRLower,
	   CTRNegative,BidRaise,BidLower, MinBid, MaxBid ,
	   ISNULL(bst.ACoSCutOff,0) BestACoSCutOff,CAST(ISNULL(bst.ImpressionCutOff,0) AS decimal) BestImpressionCutOff,
	   ISNULL(bst.ID,0) BSTId
	   from Formula as f join Campaigns as c on f.FormulaID=c.FormulaID
	   LEFT JOIN BestSearchTermRequest bst ON bst.CampaignId=c.RecordID AND bst.MppUserId=@UserID
	    where f.FormulaID = @Id
END
END

