
IF NOT EXISTS(SELECT * FROM SYS.COLUMNS WHERE Name = N'TempSKUCheck' AND ObJECT_ID = OBJECT_ID('Campaigns'))
BEGIN
	ALTER TABLE Campaigns
		ADD TempSKUCheck bit NOT NULL DEFAULT(0)
END
GO