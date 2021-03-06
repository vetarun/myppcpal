IF EXISTS(
        SELECT*
        FROM sys.objects
        WHERE object_id = OBJECT_ID(N'[dbo].[Sbsp_RevertKeyUpdates]')

            AND type IN (
                N'P'
				, N'PC'
				) 
		)
	DROP PROCEDURE[dbo].[Sbsp_RevertKeyUpdates]
GO 
CREATE procedure [dbo].[Sbsp_RevertKeyUpdates]
(
@KeyID bigint ,
@UserID bigint ,
@CampID bigint ,
@ReportID int ,
@ReasonID int,
@AdGroupID bigint ,
@ModifiedOn smalldatetime 
)
AS
BEGIN 

declare @Bid varchar

set @Bid = (select OldValue  from OptimizeKeyLog where KeywordID=@KeyID and  CampaignID=@CampID  and ReportID=@ReportID and adgroupID = @AdGroupID and ReasonID=@ReasonID  )
if @ReasonID < 5 
begin 
update OptimizeKeyLog set NewValue = OldValue ,OldValue=NewValue , ManuallyChangedOn = getdate() where KeywordID=@KeyID and KeywordID=@KeyID and  CampaignID=@CampID and ReportID=@ReportID and adgroupID = @AdGroupID and ReasonID=@ReasonID and cast(CreatedOn as date ) = cast(@ModifiedOn as date )
update Keywords set Status='enabled' , ManuallyChangedOn = getdate() where RecordID=@KeyID and  CampaignID=@CampID and adgroupID = @AdGroupID 
end 
else

if @ReasonID >4 and @ReasonID <9
begin 
update OptimizeKeyLog set NewValue = OldValue ,OldValue=NewValue  , ManuallyChangedOn = getdate() where KeywordID = @KeyID and  CampaignID=@CampID and adgroupID = @AdGroupID  and ReportID=@ReportID  and ReasonID=@ReasonID and cast(CreatedOn as date ) = cast(@ModifiedOn as date )
update Keywords set Bid= @Bid , ManuallyChangedOn = getdate() where RecordID= @KeyID and  CampaignID=@CampID and adgroupID = @AdGroupID  
end 
--else if @Reason  = 6 
--begin 
--update OptimizeKeyLog set NewValue = OldValue ,OldValue=NewValue where KeywordID=@KeyID and modifiedon = (select max(modifiedon) from OptimizeKeyLog where KeywordID = @KeyID)
--end 
--else if @Reason  = 7
--begin 
--update OptimizeKeyLog set NewValue = OldValue ,OldValue=NewValue where KeywordID=@KeyID and modifiedon = (select max(modifiedon) from OptimizeKeyLog where KeywordID = @KeyID)
--end 
--else if @Reason  = 8 
--begin 
--update OptimizeKeyLog set NewValue = OldValue ,OldValue=NewValue where KeywordID=@KeyID and modifiedon = (select max(modifiedon) from OptimizeKeyLog where KeywordID = @KeyID)
--end 
else 
begin 
update OptimizeKeyLog set NewValue = OldValue ,OldValue=NewValue ,ManuallyChangedOn = getdate()  where KeywordID=@KeyID and  CampaignID=@CampID and adgroupID = @AdGroupID  and ReportID=@ReportID  and ReasonID=@ReasonID and cast(CreatedOn as date ) = cast(@ModifiedOn as date )
update SearchtermInventory set   IsNegKeyword=0  where RecordID= @KeyID 
update Keywords set  ManuallyChangedOn = getdate() where RecordID=@KeyID and  CampaignID=@CampID and adgroupID = @AdGroupID 
end 
end 
--else if @Reason  = 10 
--begin 
--update OptimizeKeyLog set NewValue = OldValue ,OldValue=NewValue where KeywordID=@KeyID and modifiedon = (select max(modifiedon) from OptimizeKeyLog where KeywordID = @KeyID)
--end 
--else if @Reason  = 11 
--begin 
--update OptimizeKeyLog set NewValue = OldValue ,OldValue=NewValue where KeywordID=@KeyID and modifiedon = (select max(modifiedon) from OptimizeKeyLog where KeywordID = @KeyID)
--end 
--else
--begin 
--update OptimizeKeyLog set NewValue = OldValue ,OldValue=NewValue where KeywordID=@KeyID and modifiedon = (select max(modifiedon) from OptimizeKeyLog where KeywordID = @KeyID)

--end 
--end 
--else if @Reason  = 10 
--begin 
--update OptimizeKeyLog set NewValue = OldValue ,OldValue=NewValue where KeywordID=@KeyID and modifiedon = (select max(modifiedon) from OptimizeKeyLog where KeywordID = @KeyID)
--end 
--else if @Reason  = 11 
--begin 
--update OptimizeKeyLog set NewValue = OldValue ,OldValue=NewValue where KeywordID=@KeyID and modifiedon = (select max(modifiedon) from OptimizeKeyLog where KeywordID = @KeyID)
--end 
--else
--begin 
--update OptimizeKeyLog set NewValue = OldValue ,OldValue=NewValue where KeywordID=@KeyID and modifiedon = (select max(modifiedon) from OptimizeKeyLog where KeywordID = @KeyID)

--end 
--end 