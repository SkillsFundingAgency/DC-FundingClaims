﻿
CREATE ROLE [DataProcessor] AUTHORIZATION [dbo]
GO

-- Grant access rights to a specific schema in the database

GO
GRANT 
	DELETE, 
	EXECUTE, 
	INSERT, 
	REFERENCES, 
	SELECT, 
	UPDATE, 
	VIEW DEFINITION 
ON SCHEMA::[Draft]
	TO [DataProcessor]
GO
GRANT 
	DELETE, 
	EXECUTE, 
	INSERT, 
	REFERENCES, 
	SELECT, 
	UPDATE, 
	VIEW DEFINITION 
ON SCHEMA::[Static]
	TO [DataProcessor]
GO

GRANT 
	DELETE, 
	EXECUTE, 
	INSERT, 
	REFERENCES, 
	SELECT, 
	UPDATE, 
	VIEW DEFINITION 
ON SCHEMA::[Reference]
	TO [DataProcessor]
GO