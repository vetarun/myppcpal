
IF NOT EXISTS(SELECT * FROM SYS.COLUMNS WHERE Name = N'RefStatusCount' AND ObJECT_ID = OBJECT_ID('ReportType'))
BEGIN
    ALTER TABLE ReportType
		ADD RefStatusCount int default 0
END
GO