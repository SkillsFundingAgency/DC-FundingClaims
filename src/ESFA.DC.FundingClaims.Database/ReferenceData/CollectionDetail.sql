
BEGIN

	DECLARE @SummaryOfChanges_CollectionDetail TABLE ([CollectionId] INT, [Action] VARCHAR(100));

	MERGE INTO [dbo].CollectionDetail AS Target
	USING (
			SELECT NewRecords.[CollectionId],NewRecords.[CollectionName],NewRecords.[CollectionYear],NewRecords.[DisplayTitle],
			NewRecords.[SubmissionOpenDateUtc],NewRecords.[SubmissionCloseDateUtc],
				NewRecords.[SignatureCloseDateUtc],NewRecords.[RequiresSignature],NewRecords.[CollectionCode],
				NewRecords.[SummarisedPeriodFrom],NewRecords.[SummarisedPeriodTo], NewRecords.[SummarisedReturnPeriod],
				NewRecords.[HelpdeskOpenDateUtc],NewRecords.[DateTimeUpdatedUtc], NewRecords.[UpdatedBy]
			FROM 
			(
				  SELECT 6 AS [CollectionId], N'1819-YearEnd' AS [CollectionName],1819 as CollectionYear, N'Year end forecast 2018/19' as DisplayTitle, N'2019-06-09T08:00:00.000' AS [SubmissionOpenDateUtc],N'2019-06-12T18:00:00.000' AS [SubmissionCloseDateUtc],NULL [SignatureCloseDateUtc],0 AS [RequiresSignature],N'FC02' AS [CollectionCode],201808 AS [SummarisedPeriodFrom],201905 AS [SummarisedPeriodTo], N'R10' AS [SummarisedReturnPeriod], N'2019-06-08T08:00:00.000' AS [HelpdeskOpenDateUtc], GetUtcDate() AS [DateTimeUpdatedUtc], 'Lynne Burdon' As [UpdatedBy]
			UNION SELECT 10 AS [CollectionId],N'1819-Final'   AS [CollectionName],1819 as CollectionYear, N'Final funding claim 2018/19' as DisplayTitle,N'2019-10-21T08:00:00.000' AS [SubmissionOpenDateUtc],N'2019-10-29T17:00:00.000' AS [SubmissionCloseDateUtc],N'2019-10-30T17:00:00.000' [SignatureCloseDateUtc],1 AS [RequiresSignature],N'FC03' AS [CollectionCode],201808 AS [SummarisedPeriodFrom],201907 AS [SummarisedPeriodTo], N'R14' AS [SummarisedReturnPeriod], N'2019-10-20T08:00:00.000' AS [HelpdeskOpenDateUtc], GetUtcDate() AS [DateTimeUpdatedUtc], 'Lynne Burdon' As [UpdatedBy]
			UNION SELECT 96 AS [CollectionId],N'1920-MidYear'   AS [CollectionName],1920 as CollectionYear, N'Mid Year (R06) Funding Claim 2019 to 2020'  as DisplayTitle,N'2020-02-10T09:00:00.000' AS [SubmissionOpenDateUtc],N'2020-02-13T17:00:00.000' AS [SubmissionCloseDateUtc],NULL [SignatureCloseDateUtc],0 AS [RequiresSignature],N'FC01' AS [CollectionCode],201908 AS [SummarisedPeriodFrom],202001 AS [SummarisedPeriodTo], N'R06' AS [SummarisedReturnPeriod], N'2020-02-09T09:00:00.000' AS [HelpdeskOpenDateUtc], GetUtcDate() AS [DateTimeUpdatedUtc], 'Lynne Burdon' As [UpdatedBy]

			) AS NewRecords
		  )
		AS Source([CollectionId],[CollectionName],[CollectionYear],[DisplayTitle],[SubmissionOpenDateUtc],[SubmissionCloseDateUtc],[SignatureCloseDateUtc],[RequiresSignature],
				[CollectionCode],[SummarisedPeriodFrom],[SummarisedPeriodTo], [SummarisedReturnPeriod],[HelpdeskOpenDateUtc],[DateTimeUpdatedUtc],[UpdatedBy])
			ON Target.[CollectionId] = Source.[CollectionId]
		WHEN NOT MATCHED BY TARGET THEN INSERT([CollectionId],[CollectionName],[CollectionYear],[DisplayTitle],[SubmissionOpenDateUtc],[SubmissionCloseDateUtc],[SignatureCloseDateUtc],[RequiresSignature],[CollectionCode],[SummarisedPeriodFrom],[SummarisedPeriodTo], [SummarisedReturnPeriod],[HelpdeskOpenDateUtc],[DateTimeUpdatedUtc],[UpdatedBy]) 
									   VALUES ([CollectionId],[CollectionName],[CollectionYear],[DisplayTitle],[SubmissionOpenDateUtc],[SubmissionCloseDateUtc],[SignatureCloseDateUtc],[RequiresSignature],[CollectionCode],[SummarisedPeriodFrom],[SummarisedPeriodTo], [SummarisedReturnPeriod],[HelpdeskOpenDateUtc],[DateTimeUpdatedUtc],[UpdatedBy])
		OUTPUT Inserted.[CollectionId],$action INTO @SummaryOfChanges_CollectionDetail([CollectionId],[Action])
	;

		DECLARE @AddCount_C INT, @UpdateCount_C INT, @DeleteCount_C INT
		SET @AddCount_C  = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_CollectionDetail WHERE [Action] = 'Insert' GROUP BY Action),0);
		SET @UpdateCount_C = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_CollectionDetail WHERE [Action] = 'Update' GROUP BY Action),0);
		SET @DeleteCount_C = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_CollectionDetail WHERE [Action] = 'Delete' GROUP BY Action),0);

		RAISERROR('		         %s - Added %i - Update %i - Delete %i',10,1,'        Collection', @AddCount_C, @UpdateCount_C, @DeleteCount_C) WITH NOWAIT;
END
GO



BEGIN

UPDATE [dbo].CollectionDetail SET [HelpdeskOpenDateUtc] = DATEADD(day,-1,[SubmissionOpenDateUtc]) WHERE [HelpdeskOpenDateUtc] = CONVERT(DATETIME, '01 JAN 1900');
RAISERROR('		   Update CollectionDetail [HelpdeskOpenDateUtc] : %i Records updated.',10,1,@@ROWCOUNT) WITH NOWAIT;

END
GO


BEGIN

UPDATE [dbo].CollectionDetail SET [DateTimeUpdatedUtc] = GETUTCDATE() WHERE [DateTimeUpdatedUtc] = CONVERT(DATETIME, '01 JAN 1900');
RAISERROR('		   Update CollectionDetail [DateTimeUpdatedUtc] : %i Records updated.',10,1,@@ROWCOUNT) WITH NOWAIT;

END
GO



BEGIN

UPDATE [dbo].CollectionDetail SET [UpdatedBy] = 'Lynne Burdon' WHERE [UpdatedBy] = 'DataMigration';
RAISERROR('		   Update CollectionDetail [UpdatedBy] : %i Records updated.',10,1,@@ROWCOUNT) WITH NOWAIT;

END
GO