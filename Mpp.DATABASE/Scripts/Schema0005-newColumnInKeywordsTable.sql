IF NOT EXISTS(SELECT * FROM SYS.COLUMNS WHERE Name = N'ManuallyChangedOn' AND ObJECT_ID = OBJECT_ID('Keywords'))
BEGIN
    ALTER TABLE Keywords
		ADD ManuallyChangedOn smalldatetime  
END
GO