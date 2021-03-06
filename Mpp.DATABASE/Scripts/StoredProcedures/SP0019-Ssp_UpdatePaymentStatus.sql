IF EXISTS(
        SELECT*
        FROM sys.objects
        WHERE object_id = OBJECT_ID(N'[dbo].[Ssp_UpdatePaymentStatus]')

            AND type IN (
                N'P'
				, N'PC'
				) 
		)
	DROP PROCEDURE[dbo].[Ssp_UpdatePaymentStatus]
GO 
CREATE PROCEDURE [dbo].[Ssp_UpdatePaymentStatus]
@custid varchar(500),
@amount decimal(10,2),
@amount_off decimal(10,2),
@couponid   varchar(200),
@discount   decimal(10,2),
@startdate  smalldatetime,
@enddate    smalldatetime,
@status     bit,
@date       datetime
AS
BEGIN
DECLARE @mppuserid int
DECLARE @billid int
DECLARE @aff_id int = NULL
DECLARE @alertID int
DECLARE @Attempt int
DECLARE @useralertId int
DECLARE @commision decimal(10,2)
DECLARE @topenddate datetime

SET @commision=
CASE 
    WHEN @amount=0 OR (@amount_off=@amount) THEN 0.00
	ELSE (((@amount_off/@amount)+.30)-1.00)*@amount
	END

IF @commision<=0
BEGIN
SET @commision=0.00;
END

SET @mppuserid = ISNULL((SELECT MppUserID from MppUser WHERE stp_custId=@custid),0)
IF(@mppuserid !=0)
BEGIN
	SET @billid = ISNULL((SELECT TOP 1 ID from Billing WHERE MppUserId=@mppuserid and DATEADD(dd, 0, DATEDIFF(dd, 0, PlanStartDate))=DATEADD(dd, 0, DATEDIFF(dd, 0, @startDate)) and DATEADD(dd, 0, DATEDIFF(dd, 0, PlanEndDate))=DATEADD(dd, 0, DATEDIFF(dd, 0, @enddate))),0)
IF @couponid IS NOT NULL  
BEGIN
  SET @aff_id = (SELECT AffiliateID from AffiliationCode WHERE AffiliateCode = @couponid)
END
IF(@billid !=0)
BEGIN
    SET @Attempt = ISNULL((SELECT PaymentAttmepts from Billing 
	WHERE MppUserId=@mppuserid and PlanStartDate=@startDate and PlanEndDate=@enddate),0)
	IF(@status = 0)
	BEGIN
	     SET @Attempt = @Attempt + 1;
	END
	ELSE
	BEGIN
	SET @Attempt = 0;
	END
	
	IF(@Attempt>2)
	BEGIN
	DELETE from Billing WHERE ID = @billid
	UPDATE MppUser set stp_cardId = null, PlanStatus= 0 WHERE MppUserId=@mppuserid
    END
	ELSE
	BEGIN
	UPDATE Billing set AffiliateID=@aff_id,
	                    Amount=@amount,
						Amount_off=@amount_off,
                      	Discount_off=@discount,
						CurrentPaymentStatus=@status,
						PaymentAttmepts=@Attempt,
						ModifiedOn= GETDATE(),
						AffiliateEarning=@commision
	
   WHERE ID = @billid
	END
END
ELSE
BEGIN
    SET @topenddate = (SELECT TOP 1 PlanEndDate FROM Billing WHERE MppUserId=@mppuserid ORDER BY ID DESC)
	IF(DATEADD(dd, -2, @topenddate) <= @startdate) /* NOT ALLOW DUPLICATE ENTRIES */
	BEGIN
		IF(@amount>0)
		BEGIN
		IF(@status = 0)
		BEGIN
			SET @alertID = 5
		END
		ELSE
		BEGIN IF(@status = 1)
			SET @alertID = 4
		END
		END
		SET @useralertId = ISNULL((SELECT TOP 1 Id FROM UserAlerts WHERE MppUserId=@mppuserid and AlertDate=@date and AlertID = @AlertID),0)
		IF(@useralertId = 0 and @AlertID > 0)
		BEGIN
			INSERT INTO UserAlerts(MppUserId, AlertID, AlertDate,CreatedOn) VALUES(@MppUserid,@alertID,@date,GETDATE())
		END
		INSERT INTO Billing(MppUserId,AffiliateID,Amount,Amount_off,Discount_off,PlanStartDate,PlanEndDate,CurrentPaymentStatus,PaymentAttmepts,CreatedOn,AffiliateEarning)
		VALUES(@mppuserid,@aff_id,@amount,@amount_off,@discount,@StartDate,@EndDate,@status,1,GETDATE(),@commision)
    END
END
END
END
