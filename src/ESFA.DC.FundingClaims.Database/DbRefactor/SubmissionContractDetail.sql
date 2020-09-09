IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'FundingClaimMaxContractValues')
                AND NOT EXISTS (SELECT TOP 1 * FROM  dbo.[SubmissionContractDetail]))
BEGIN

INSERT INTO [SubmissionContractDetail]
      ([FundingStreamPeriodCode]
      ,[ContractValue]
      ,[SubmissionId])

SELECT    
	   [FundingStreamPeriodCode]
      ,[MaximumContractValue]
      ,cv.[SubmissionId]
  FROM [dbo].[FundingClaimMaxContractValues] cv
  Join dbo.Submission s
  on s.SubmissionId = cv.SubmissionId

  END