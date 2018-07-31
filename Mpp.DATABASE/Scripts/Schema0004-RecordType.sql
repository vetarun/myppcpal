IF EXISTS(SELECT * FROM SYS.COLUMNS WHERE Name = N'RecordName' AND ObJECT_ID = OBJECT_ID('RecordType'))
BEGIN
	
	IF NOT EXISTS(SELECT 1 FROM RecordType WHERE RecordName = 'Campaigns')
	BEGIN
	 INSERT INTO RecordType VALUES(1,'Campaigns', GETDATE())
	END
	
	IF NOT EXISTS(SELECT 1 FROM RecordType WHERE RecordName = 'Campaigns')
	BEGIN
	 INSERT INTO RecordType VALUES(2,'Campaigns', GETDATE())
	END
	
	IF NOT EXISTS(SELECT 1 FROM RecordType WHERE RecordName = 'Keywords')
	BEGIN
	 INSERT INTO RecordType VALUES(3,'Keywords', GETDATE())
	END
	
	IF NOT EXISTS(SELECT 1 FROM RecordType WHERE RecordName = 'SearchTerm')
	BEGIN
	 INSERT INTO RecordType VALUES(4,'SearchTerm', GETDATE())
	END
END
GO