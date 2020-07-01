
DECLARE @SummaryOfChanges_submissionTypes TABLE ([EventId] INT, [Action] VARCHAR(100));

MERGE INTO [static].[submissionTypes] AS Target
USING (VALUES
		(1, '1819-MidYear'),
		(2, '1819-YearEnd'),
		(3, '1819-Final')
	  )
	AS Source([Id], [Name])
		ON Target.[Id] = Source.[Id]
	WHEN MATCHED 
			AND EXISTS 
				(		SELECT Target.[Name]
					EXCEPT 
						SELECT Source.[Name]
							   
				)
		  THEN UPDATE SET Target.[Name] = Source.[Name]

	WHEN NOT MATCHED BY TARGET THEN INSERT([Id], [Name]) 
								   VALUES ([Id], [Name])
	WHEN NOT MATCHED BY SOURCE THEN DELETE
	OUTPUT Inserted.[Id],$action INTO @SummaryOfChanges_submissionTypes([EventId],[Action])
;

	DECLARE @AddCount_JTG INT, @UpdateCount_JTG INT, @DeleteCount_JTG INT
	SET @AddCount_JTG  = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_submissionTypes WHERE [Action] = 'Insert' GROUP BY Action),0);
	SET @UpdateCount_JTG = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_submissionTypes WHERE [Action] = 'Update' GROUP BY Action),0);
	SET @DeleteCount_JTG = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_submissionTypes WHERE [Action] = 'Delete' GROUP BY Action),0);

	RAISERROR('		               %s - Added %i - Update %i - Delete %i',10,1,'SubmissionTypes', @AddCount_JTG, @UpdateCount_JTG, @DeleteCount_JTG) WITH NOWAIT;

