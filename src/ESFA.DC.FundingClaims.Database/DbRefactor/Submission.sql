IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'FundingClaimsSubmissionFile')
                 AND NOT EXISTS (SELECT TOP 1 * FROM  dbo.Submission))
BEGIN

    INSERT INTO Submission([SubmissionId]
          ,[UKPRN]
          ,[CollectionId]
          ,[Declaration]
          ,[Version]
          ,[IsSigned]
          ,[OrganisationIdentifier]
          ,[SubmittedBy]
          ,[IsSubmitted]
          ,[SubmittedDateTimeUtc]
          ,[SignedOnDateTimeUtc]
          ,[CreatedBy]
          ,[CreatedDateTimeUtc])
  
      SELECT 
	       [SubmissionId]
          ,[UKPRN]
          ,CASE CollectionPeriod When 'FC02' THEN 6 WHEN 'FC03' Then 10 WHEN 'FC01' THEN 96  END
          ,[Declaration]
          ,[Version]
          ,[IsSigned]
          ,[OrganisationIdentifier]
          ,[UpdatedBy]
          ,1
          ,UpdatedOn
          ,NULL
          ,[UpdatedBy]
          ,[UpdatedOn]
      FROM [dbo].[FundingClaimsSubmissionFile]

  END