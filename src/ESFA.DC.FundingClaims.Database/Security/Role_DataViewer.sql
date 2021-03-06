﻿
CREATE ROLE [DataViewer] AUTHORIZATION [dbo]
GO

-- Grant access rights to a specific schema in the database

GO

GRANT 
	REFERENCES, 
	SELECT, 
	VIEW DEFINITION 
ON SCHEMA::[Reference]
	TO [DataViewer]
GO
