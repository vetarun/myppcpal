IF NOT EXISTS(SELECT
  			*
  		FROM
  			INFORMATION_SCHEMA.TABLES
  		WHERE
  			TABLE_NAME = 'PopUpAlert'
			)
BEGIN
    CREATE TABLE PopUpAlert (
    ID int primary key identity (1,1),
    MppUserID int ,
    PopUpId int )
	END