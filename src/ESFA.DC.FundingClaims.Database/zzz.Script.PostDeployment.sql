/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
SET NOCOUNT ON; 

GO
-- Set ExtendedProperties fro DB.
	:r .\z.ExtendedProperties.sql
GO


RAISERROR('		   Ref Data',10,1) WITH NOWAIT;
	:r .\ReferenceData\CollectionDetail.sql
	:r .\DbRefactor\Submission.sql
	:r .\DbRefactor\DeliverableCode.sql
	:r .\DbRefactor\SubmissionContractDetail.sql
	:r .\DbRefactor\SubmissionValue.sql
GO

RAISERROR('		   Update User Account Passwords',10,1) WITH NOWAIT;
GO

REVOKE REFERENCES ON SCHEMA::[dbo] FROM [DataProcessor];
REVOKE REFERENCES ON SCHEMA::[dbo] FROM [DataViewer];
GO

RAISERROR('		         FundingClaims RO User',10,1) WITH NOWAIT;
ALTER USER [FundingClaims_RO_User] WITH PASSWORD = N'$(ROUserPassword)';
GO

RAISERROR('		         FundingClaims RW User',10,1) WITH NOWAIT;
ALTER USER [FundingClaims_RW_User] WITH PASSWORD = N'$(RWUserPassword)';
GO

RAISERROR('Completed',10,1) WITH NOWAIT;
GO
