IF NOT EXISTS(SELECT
  			*
  		FROM
  			INFORMATION_SCHEMA.TABLES
  		WHERE
  			TABLE_NAME = 'PopUp'
			)	
	CREATE TABLE [PopUp] (
	ID int primary key ,
	PopUpType varchar 
	 )
