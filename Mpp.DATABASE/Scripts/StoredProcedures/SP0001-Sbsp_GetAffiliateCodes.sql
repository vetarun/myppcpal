IF EXISTS(
        SELECT*
        FROM sys.objects
        WHERE object_id = OBJECT_ID(N'[dbo].[Sbsp_GetAffiliateCodes]')

            AND type IN (
                N'P'
				, N'PC'
				) 
		)
	DROP PROCEDURE[dbo].[Sbsp_GetAffiliateCodes]
GO 
CREATE PROCEDURE [dbo].[Sbsp_GetAffiliateCodes] 
	@userId int,
	@srchTerm varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

  select af.AffiliateID,af.AffiliateCode,ISNULL(af.Amount,0) as Amount,af.Duration,
                                ISNULL(af.Percentile_off,0) as Percent_off,
                                ISNULL(af.Maxredem,0) as Maxredeem,af.Redeemby, count(bl.MppUserId) as Subscribers,
                                ISNULL(SUM(bl.Amount_off),0) as Sales,
                                ISNULL(SUM(bl.Amount),0) as PreOffSale ,usr.FirstName+' '+usr.LastName as CreatedBy,
								0 AssignId,
								COUNT(bl.ID) Redeem,
						        SUM(ISNULL(bl.AffiliateEarning,0.0)) AS Commision
                                from AffiliationCode  as af 
                                left outer join Billing as bl on af.AffiliateID = bl.AffiliateID
                                join MppAdmin usr on usr.MppAdminID=af.CreatedBy
                                where CreatedBy=@userId and NOT EXISTS(SELECT * FROM AffiliateAssignedCodes where CouponId=af.AffiliateID) and af.AffiliateCode like @srchTerm+'%'
                                group by af.AffiliateID,af.AffiliateCode,af.Amount,af.Duration,af.Percentile_off,af.Maxredem,
								af.Redeemby,usr.FirstName,usr.LastName
UNION
select af.AffiliateID,af.AffiliateCode,ISNULL(af.Amount,0) as Amount,af.Duration,
                                ISNULL(af.Percentile_off,0) as Percent_off,
                                ISNULL(af.Maxredem,0) as Maxredeem,af.Redeemby, count(bl.MppUserId) as Subscribers,
                                ISNULL(SUM(bl.Amount_off),0) as Sales,
                                ISNULL(SUM(bl.Amount),0) as PreOffSale ,usr.FirstName+' '+usr.LastName as CreatedBy,
								0 AssignId,
							    COUNT(bl.ID) Redeem,
							    SUM(ISNULL(bl.AffiliateEarning,0.0)) AS Commision
                                from AffiliationCode  as af 
							    JOIN AffiliateAssignedCodes afas on af.AffiliateID=afas.CouponId and afas.UserId=@userId
                                left outer join Billing as bl on af.AffiliateID = bl.AffiliateID
                                join MppAdmin usr on usr.MppAdminID=af.CreatedBy
						         where af.AffiliateCode like @srchTerm+'%'
                                group by af.AffiliateID,af.AffiliateCode,af.Amount,af.Duration,af.Percentile_off,
								af.Maxredem,af.Redeemby,usr.FirstName,usr.LastName

END
