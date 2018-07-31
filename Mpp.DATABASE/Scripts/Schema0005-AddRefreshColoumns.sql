IF NOT EXISTS(SELECT * FROM SYS.COLUMNS WHERE Name = N'RefreshedOn' AND ObJECT_ID = OBJECT_ID('ReportType'))
BEGIN
     Alter Table ReportType add RefreshedOn datetime 
END