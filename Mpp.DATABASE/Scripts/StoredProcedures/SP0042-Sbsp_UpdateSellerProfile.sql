IF EXISTS(
        SELECT*
        FROM sys.objects
        WHERE object_id = OBJECT_ID(N'[dbo].[Sbsp_UpdateSellerProfile]')

            AND type IN (
                N'P'
				, N'PC'
				) 
		)
	DROP PROCEDURE[dbo].[Sbsp_UpdateSellerProfile]
GO 
CREATE PROCEDURE [dbo].[Sbsp_UpdateSellerProfile]
(
    @mppUserId   int,
	@profileId   varchar(100),
	@sellerStringId varchar(100),
	@date   datetime,
	@modifiedby varchar(30)
)
AS
BEGIN
    IF EXISTS(SELECT 1 from MppUser where MppUserID!=@mppUserId and ProfileId=@profileId and sellerStringId=@sellerStringId)
	BEGIN
		RAISERROR('Seller is already used in trail account.',16,1)
	END
	ELSE
	BEGIN
		UPDATE MppUser set ProfileId=@profileId,sellerStringId=@sellerStringId,ProfileAccess=1,ModifiedBy=@modifiedby where MppUserId=@mppUserId
		Exec Sbsp_SetReportDatesForUserId @mppUserId, @date 
    END
END
