IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'FundingClaimsSubmissionValues')
                 AND NOT EXISTS (SELECT TOP 1 * FROM  dbo.[SubmissionValue]))
BEGIN

  INSERT INTO [dbo].[SubmissionValue]
	  ([SubmissionId]
      ,[DeliverableCodeId]
      ,[DeliveryToDate]
      ,[ForecastedDelivery]
      ,[ExceptionalAdjustments]
      ,[TotalDelivery]
      ,[StudentNumbers]
      ,[FundingStreamPeriodCode]
      ,[ContractAllocationNumber])
  
     SELECT [SubmissionId]
      ,c.[DeliverableCodeId]
      ,IsNull([DeliveryToDate],0)
      ,IsNull([ForecastedDelivery],0)
      ,IsNull([ExceptionalAdjustments],0)
      ,IsNull([TotalDelivery],0)
      ,IsNull([StudentNumbers],0)
      ,sv.[FundingStreamPeriodCode]
      ,[ContractAllocationNumber]
  FROM 	 [dbo].[FundingClaimsSubmissionValues] sv
  JOIN dbo.[DeliverableCode] c
  on c.[DeliverableCodeId] = sv.DeliverableCode
  And c.[FundingStreamPeriodCode] = sv.[FundingStreamPeriodCode]
  Where contractAllocationNumber is not null



  END