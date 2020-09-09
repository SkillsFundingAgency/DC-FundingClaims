IF (        EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'FundingClaimsSubmissionValues')
                AND NOT EXISTS (SELECT TOP 1 * FROM  FundingStreamPeriodDeliverableCode))

BEGIN

    INSERT INTO [dbo].[FundingStreamPeriodDeliverableCode]
		([DeliverableCode]
		,[Description]
      ,[FundingStreamPeriodCode] )

SELECT DISTINCT
	[DeliverableCode]
      ,[DeliverableDescription]
	  ,[FundingStreamPeriodCode]
  FROM [dbo].[FundingClaimsSubmissionValues]
  Where CollectionPeriod IN ('FC01','FC02','FC03')

  END