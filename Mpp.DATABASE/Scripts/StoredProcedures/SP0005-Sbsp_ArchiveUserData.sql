
IF EXISTS(
        SELECT*
        FROM sys.objects
        WHERE object_id = OBJECT_ID(N'[dbo].[Sbsp_ArchiveUserData]')

            AND type IN (
                N'P'
				, N'PC'
				) 
		)
	DROP PROCEDURE[dbo].[Sbsp_ArchiveUserData]
GO 
CREATE procedure [dbo].[Sbsp_ArchiveUserData] 
AS
BEGIN
 update  MppUser set IsArchive = 1  where 
   IsArchive=0 AND (
  (GETDATE()>DATEADD(day, 30,TrailEndDate) and ProfileAccess =0) 
OR
 ( DATEADD(day, 90,TrailEndDate)<GETDATE() and stp_cardId IS NULL and PlanStatus!=1 ) 
 OR 
 (DATEADD(day, 90,ModifiedOn)<GETDATE() and stp_cardId IS NULL and PlanStatus!=1  )
 OR
 (GETDATE()>TrailEndDate AND Active=0 and EXISTS(select * from MppUserActivation where MppUserID=MppUser.MppUserID))
 )
select Email, ActivationCode,FirstName,LastName from MppUser join MppUserActivation  on MppUser.MppUserID=MppUserActivation.MppUserID where  DATEADD(HOUR, 1, MppUserActivation.CreatedOn) < GETDATE() and TrailEndDate > GETDATE()
END
