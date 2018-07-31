IF EXISTS(
        SELECT*
        FROM sys.objects
        WHERE object_id = OBJECT_ID(N'[dbo].[Sbsp_GetAdminDashboard]')

            AND type IN (
                N'P'
				, N'PC'
				) 
		)
	DROP PROCEDURE[dbo].[Sbsp_GetAdminDashboard]
GO 
CREATE PROCEDURE [dbo].[Sbsp_GetAdminDashboard]
( @date smallDateTime
)
as
BEGIN  
  DECLARE @CurrentMonthlySales decimal
  DECLARE @TotalYearlySales decimal
  DECLARE @LastMonthSales decimal
  DECLARE @Inactive int
  DECLARE @PendingAccess int
  DECLARE @TrailEnd int
  DECLARE @Unsubscribed int
  DECLARE @ActivePaid int
  DECLARE @ActiveTrail int
   DECLARE @NewlyPaid int
   begin
  SET @CurrentMonthlySales = (Select ISNULL(sum(Amount_off), 0) from MppUser join Billing on MppUser.MppUserID = Billing.MppUserId where
     CurrentPaymentStatus =1 AND Amount_off != 0   AND stp_cardId is not null and MONTH(billing.CreatedOn) = MONTH(GETDATE()))

  SET @TotalYearlySales = (Select ISNULL(sum(Amount_off), 0)from MppUser join Billing on MppUser.MppUserID = Billing.MppUserId  where 
    CurrentPaymentStatus =1 AND Amount_off != 0   AND stp_cardId is not null and YEAR(billing.CreatedOn) = YEAR(GETDATE()))

  SET @LastMonthSales = (Select ISNULL(sum(Amount_off), 0) from MppUser join Billing on MppUser.MppUserID = Billing.MppUserId  where 
    CurrentPaymentStatus =1 AND Amount_off != 0   AND stp_cardId is not null and month(billing.CreatedOn)= MONTH(DATEADD(MONTH,-1,GETDATE())))

  set @ActivePaid =(select count(*) from MppUser u join Billing b on u.MppUserID= b.MppUserId  
where PlanID not in  (1,7)  and IsArchive=0 and Active=1 and stp_cardId is not null and PlanStatus=1 and b.CreatedOn=(SELECT MAX(CreatedOn) 
from Billing where MppUserId=u.MppUserID)
and b.Amount_off>0)

set @ActiveTrail=(select count(*) from MppUser where PlanID = 1 AND Active=1 and ProfileAccess !=0 and IsArchive=0 and PlanStatus = 1 )
  
  SET @Inactive = (Select count(*) from MppUser join MppUserActivation on MppUser.MppUserID = MppUserActivation.MppUserID  where
   IsArchive=0 and Active=0 )
 SET @PendingAccess = (Select count(*) from MppUser  where PlanID = 1 and  active=1 and ProfileAccess =0  and IsArchive=0 ) 
  SET @TrailEnd = (Select count(*) from MppUser  where PlanID = 7 and stp_cardId IS  NULL and PlanStatus !=1 and  IsArchive=0 and
   TrailEndDate<getDate())
  SET @Unsubscribed = (Select count(*) from MppUser  where active = 1  and PlanStatus =2 and IsArchive=0  )

 
   SET @NewlyPaid = (SELECT count(*) FROM MppUser
 where MppUserID in (select u.MppUserID  FROM MppUser u join Billing b on b.MppUserId =u.MppUserID where b.MppUserId in
 (select  b.MppUserId  from  Billing b  where CurrentPaymentStatus=1 and Amount_off>0 group by b.MppUserId
  having count( b.CreatedOn)=1) and IsArchive=0 and Active=1 and PlanID not in (1,7) and PlanStatus=1 and  
  Amount_off in (SELECT sum(Amount_off ) from  Billing b  where CurrentPaymentStatus=1 and Amount_off>0 and 
   month(CreatedOn)=MONTH(getdate()) group by  b.MppUserId  having count( b.CreatedOn)=1)))
 
  SELECT @NewlyPaid as NewlyPaid , @CurrentMonthlySales as CurrentMonthlySales, @TotalYearlySales as TotalYearlySales,
   @LastMonthSales as LastMonthSales,@Inactive as Inactive, @TrailEnd as TrailEnd ,@Unsubscribed as Unsubscribed ,
    @PendingAccess as PendingAccess , @ActiveTrail as ActiveTrail , @ActivePaid as ActivePaid 
end
end