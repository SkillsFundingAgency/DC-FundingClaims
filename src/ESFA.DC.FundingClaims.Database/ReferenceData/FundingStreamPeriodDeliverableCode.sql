
BEGIN

	DECLARE @SummaryOfChanges_DeliverableCode TABLE ([DeliverableCode] INT, [Action] VARCHAR(100));

	MERGE INTO [dbo].FundingStreamPeriodDeliverableCode AS Target
	USING (
			SELECT [DeliverableCode], [Description], [FundingStreamPeriodCode]
			FROM 
			(
				 SELECT 1001 As [DeliverableCode], N'540+ hours (Band 5)' AS [Description], N'1619ED1920' AS [FundingStreamPeriodCode] 
					UNION SELECT 1002 As [DeliverableCode], N'450+ hours (Band 4a)'AS [Description], N'1619ED1920' AS [FundingStreamPeriodCode]
					UNION SELECT 1003 As [DeliverableCode], N'450 to 539 hours (Band 4b)' AS [Description], N'1619ED1920' AS [FundingStreamPeriodCode]
					UNION SELECT 1004 As [DeliverableCode], N'360 to 449 hours (Band 3)' AS [Description], N'1619ED1920' AS [FundingStreamPeriodCode]
					UNION SELECT 1005 As [DeliverableCode], N'280 to 359 hours (Band 2)' AS [Description], N'1619ED1920' AS [FundingStreamPeriodCode]
					UNION SELECT 1006 As [DeliverableCode], N'Up to 279 hours (Band 1)' AS [Description], N'1619ED1920' AS [FundingStreamPeriodCode]
					UNION SELECT 1007 As [DeliverableCode], N'540+ hours (Band 5)' AS [Description], N'1619ED1920' AS [FundingStreamPeriodCode]
					UNION SELECT 1008 As [DeliverableCode], N'450+ hours (Band 4a)' AS [Description], N'1619ED1920' AS [FundingStreamPeriodCode]
					UNION SELECT 1009 As [DeliverableCode], N'450 to 539 hours (Band 4b)' AS [Description], N'1619ED1920' AS [FundingStreamPeriodCode]
					UNION SELECT 1010 As [DeliverableCode], N'360 to 449 hours (Band 3)' AS [Description], N'1619ED1920' AS [FundingStreamPeriodCode]
					UNION SELECT 1011 As [DeliverableCode], N'280 to 359 hours (Band 2)' AS [Description], N'1619ED1920' AS [FundingStreamPeriodCode]
					UNION SELECT 1012 As [DeliverableCode], N'Up to 279 hours (Band 1)' AS [Description], N'1619ED1920' AS [FundingStreamPeriodCode]
					UNION SELECT 1013 As [DeliverableCode], N'540+ hours (Band 5)' AS [Description], N'1619ED1920' AS [FundingStreamPeriodCode]
					UNION SELECT 1014 As [DeliverableCode], N'450+ hours (Band 4a)' AS [Description], N'1619ED1920' AS [FundingStreamPeriodCode]
					UNION SELECT 1015 As [DeliverableCode], N'450 to 539 hours (Band 4b)' AS [Description], N'1619ED1920' AS [FundingStreamPeriodCode]
					UNION SELECT 1016 As [DeliverableCode], N'360 to 449 hours (Band 3)' AS [Description], N'1619ED1920' AS [FundingStreamPeriodCode]
					UNION SELECT 1017 As [DeliverableCode], N'280 to 359 hours (Band 2)' AS [Description], N'1619ED1920' AS [FundingStreamPeriodCode]
					UNION SELECT 1018 As [DeliverableCode], N'Up to 279 hours (Band 1)' AS [Description], N'1619ED1920' AS [FundingStreamPeriodCode]
					UNION SELECT 1019 As [DeliverableCode], N'540+ hours (Band 5)' AS [Description], N'1619ED1920' AS [FundingStreamPeriodCode]
					UNION SELECT 1020 As [DeliverableCode], N'450+ hours (Band 4a)' AS [Description], N'1619ED1920' AS [FundingStreamPeriodCode]
					UNION SELECT 1021 As [DeliverableCode], N'450 to 539 hours (Band 4b)' AS [Description], N'1619ED1920' AS [FundingStreamPeriodCode]
					UNION SELECT 1022 As [DeliverableCode], N'360 to 449 hours (Band 3)' AS [Description], N'1619ED1920' AS [FundingStreamPeriodCode]
					UNION SELECT 1023 As [DeliverableCode], N'280 to 359 hours (Band 2)' AS [Description], N'1619ED1920' AS [FundingStreamPeriodCode]
					UNION SELECT 1024 As [DeliverableCode], N'Up to 279 hours (Band 1)' AS [Description], N'1619ED1920' AS [FundingStreamPeriodCode]
					UNION SELECT 1025 As [DeliverableCode], N'Condition of Funding Removal', N'1619ED1920' AS [FundingStreamPeriodCode]


			) AS NewRecords
		  )
		AS Source([DeliverableCode], [Description], [FundingStreamPeriodCode])
			ON Target.[DeliverableCode] = Source.[DeliverableCode]
			And Target.[FundingStreamPeriodCode] = Source.[FundingStreamPeriodCode]

		WHEN NOT MATCHED BY TARGET THEN INSERT([DeliverableCode], [Description], [FundingStreamPeriodCode]) 
									   VALUES ([DeliverableCode], [Description], [FundingStreamPeriodCode])
		OUTPUT Inserted.[DeliverableCode],$action INTO @SummaryOfChanges_DeliverableCode([DeliverableCode],[Action])
	;

		DECLARE @AddCount_C INT, @UpdateCount_C INT, @DeleteCount_C INT
		SET @AddCount_C  = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_DeliverableCode WHERE [Action] = 'Insert' GROUP BY Action),0);
		SET @UpdateCount_C = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_DeliverableCode WHERE [Action] = 'Update' GROUP BY Action),0);
		SET @DeleteCount_C = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_DeliverableCode WHERE [Action] = 'Delete' GROUP BY Action),0);

		RAISERROR('		         %s - Added %i - Update %i - Delete %i',10,1,'        Collection', @AddCount_C, @UpdateCount_C, @DeleteCount_C) WITH NOWAIT;
END
GO

