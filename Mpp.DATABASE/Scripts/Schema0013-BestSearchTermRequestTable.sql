IF NOT EXISTS(SELECT
  			*
  		FROM
  			INFORMATION_SCHEMA.TABLES
  		WHERE
  			TABLE_NAME = 'BestSearchTermRequest'
			)	
CREATE  TABLE BestSearchTermRequest(
ID INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
CampaignId BIGINT NOT NULL,
MppUserId INT NOT NULL,
ACoSCutOff decimal(18,2) NOT NULL,
ImpressionCutOff INT NOT NULL,
Status INT NOT NULL,
CreatedOn datetime NULL,
ModifiedOn datetime NULL,
NewCampaignId BIGINT NULL
)

