IF EXISTS(
        SELECT*
        FROM sys.objects
        WHERE object_id = OBJECT_ID(N'[dbo].[Sbsp_UpdateFormulaForCamp]')

            AND type IN (
                N'P'
				, N'PC'
				) 
		)
	DROP PROCEDURE[dbo].[Sbsp_UpdateFormulaForCamp]
GO 
CREATE PROCEDURE [dbo].[Sbsp_UpdateFormulaForCamp]
(
    @MppUserID        int,
    @CampaignID       bigint,
	@FormulaID        int,
	@FormulaName      varchar(30),
	@AcosPause        Decimal(10,2),
    @AcosLower        Decimal(10,2),
    @AcosRaise        Decimal(10,2),
    @AcosNegative     Decimal(10,2),
    @SpendPause       Decimal(10,2),
    @SpendLower       Decimal(10,2),
    @SpendNegative    Decimal(10,2),
    @ClicksPause      Decimal(10,2),
    @ClicksLower      Decimal(10,2),
    @ClicksNegative   Decimal(10,2),
    @CTRPause         Decimal(10,2),
    @CTRLower         Decimal(10,2),
    @CTRNegative      Decimal(10,2),
    @BidRaise         Decimal(10,2),
    @BidLower         Decimal(10,2),
    @MinBid           Decimal(10,2),
	@MaxBid           Decimal(10,2),
	@ModifiedOn       DateTime,
	@isBestSrch       Bit,
	@bestSrchAcos     Decimal(10,2),
	@bestSrchImpression Decimal(10,2)
)
AS
BEGIN
       DECLARE @CFormulaID int
	 
	   Select @CFormulaID=FormulaID from Campaigns where RecordID=@CampaignID and MppUserID=@MppUserID
	   
	   IF(@FormulaName = 'Recommended' or @FormulaName = 'Conservative' or @FormulaName = 'Aggressive') /* After creating new template onwards-missed*/
	   BEGIN
	   SET @bestSrchAcos= CASE 
	                          WHEN @FormulaName = 'Recommended' THEN 20  
							  WHEN @FormulaName = 'Conservative' THEN 15 
							  WHEN @FormulaName = 'Aggressive' THEN 25 END 
	   SET @bestSrchImpression= CASE 					   
	                          WHEN @FormulaName = 'Recommended' THEN 1000
							  WHEN @FormulaName = 'Conservative' THEN 800 
							  WHEN @FormulaName = 'Aggressive' THEN 1200 END 
	   DECLARE @NFormulaID int
	       IF(@FormulaID<=3)   /* when user sets default ones */
	       BEGIN
	       set @NFormulaID = @FormulaID
	       END
		   ELSE
		   BEGIN
		   set @NFormulaID = @CFormulaID
		   END
	   Select @AcosPause=AcosPause,@AcosLower=AcosLower,@AcosRaise=AcosRaise,@AcosNegative=AcosNegative,
	   @SpendPause=SpendPause,@SpendLower=SpendLower,@SpendNegative=SpendNegative,@ClicksPause=ClicksPause
       ,@ClicksLower=ClicksLower,@ClicksNegative=ClicksNegative,@CTRPause=CTRPause,@CTRLower=CTRLower,
	   @CTRNegative=CTRNegative,@BidRaise=BidRaise,@BidLower=BidLower 
	   from Formula where FormulaID = @NFormulaID
	   END

	   
	   IF(@CFormulaID<=3)
	   BEGIN
	   Insert into Formula(FormulaName,AcosPause,AcosLower,AcosRaise,AcosNegative,SpendPause,SpendLower
                          ,SpendNegative,ClicksPause,ClicksLower,ClicksNegative,CTRPause,CTRLower,CTRNegative,
						   BidRaise,BidLower,MinBid,MaxBid,CreatedOn) 
						   values(@FormulaName,@AcosPause,@AcosLower,@AcosRaise,@AcosNegative,@SpendPause,@SpendLower,@SpendNegative,
						   @ClicksPause,@ClicksLower,@ClicksNegative,@CTRPause,@CTRLower,@CTRNegative,@BidRaise,
						   ISNULL(@BidLower,0),@MinBid,@MaxBid,Getdate())
	   set @CFormulaID = SCOPE_IDENTITY()
	   update Campaigns set FormulaID=@CFormulaID where RecordID=@CampaignID and MppUserID=@MppUserID
	   END
	   ELSE
	   BEGIN
	   update Formula set FormulaName=@FormulaName,AcosPause=@AcosPause,AcosLower=@AcosLower,AcosRaise=@AcosRaise,AcosNegative=@AcosNegative,
	   SpendPause=@SpendPause,SpendLower=@SpendLower,SpendNegative=@SpendNegative,ClicksPause=@ClicksPause
       ,ClicksLower=@ClicksLower,ClicksNegative=@ClicksNegative,CTRPause=@CTRPause,CTRLower=@CTRLower,
	   CTRNegative=@CTRNegative,BidRaise=@BidRaise,BidLower=ISNULL(@BidLower,0),MinBid=@MinBid,
	   MaxBid=@MaxBid,ModifiedOn = @ModifiedOn where FormulaID = @CFormulaID	    
	   END

	  IF(@isBestSrch=1 AND NOT EXISTS(SELECT * FROM BestSearchTermRequest WHERE CampaignId=@CampaignID AND MppUserId=@MppUserID ))
	  BEGIN
	  INSERT INTO BestSearchTermRequest(CampaignId,MppUserId,ACoSCutOff,ImpressionCutOff,Status,CreatedOn,ModifiedOn)
	  VALUES(@CampaignID,@MppUserID,@bestSrchAcos,@bestSrchImpression,0,GETDATE(),NULL)
	  END
	  ELSE
	  BEGIN
	  UPDATE BestSearchTermRequest SET ACoSCutOff=@bestSrchAcos,ImpressionCutOff=@bestSrchImpression,ModifiedOn=GETDATE()
	  WHERE CampaignId=@CampaignID AND MppUserId=@MppUserID 
	  END
	  
END

