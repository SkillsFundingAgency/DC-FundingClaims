CREATE TABLE [dbo].[CollectionDetail](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CollectionId] [int] NOT NULL,
	[CollectionYear] [int] NOT NULL,
	[CollectionName] [nvarchar](50) NOT NULL,
	[SubmissionOpenDateUtc] [datetime] NOT NULL,
	[SubmissionCloseDateUtc] [datetime] NOT NULL,
	[SignatureCloseDateUtc] [datetime] NULL,
	[RequiresSignature] [bit] NULL,
	[CollectionCode] [nvarchar](50) NOT NULL,
	[SummarisedPeriodFrom] [int] NOT NULL,
	[SummarisedPeriodTo] [int] NOT NULL,
	[SummarisedReturnPeriod] [nvarchar](10) NOT NULL,
	[HelpdeskOpenDateUtc] [datetime] NOT NULL,
	[DateTimeUpdatedUtc] [datetime] NOT NULL,
	[UpdatedBy] [varchar](50) NOT NULL,
 CONSTRAINT [PK__Collecti__3214EC0779137F1B] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ__Collecti__7DE6BC05C85E8FAD] UNIQUE NONCLUSTERED 
(
	[CollectionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[CollectionDetail] ADD  CONSTRAINT [DF__Collectio__Helpd__6B24EA82]  DEFAULT (CONVERT([datetime],'01 JAN 1900')) FOR [HelpdeskOpenDateUtc]
GO

ALTER TABLE [dbo].[CollectionDetail] ADD  CONSTRAINT [DF__Collectio__DateT__6C190EBB]  DEFAULT (CONVERT([datetime],'01 JAN 1900')) FOR [DateTimeUpdatedUtc]
GO

ALTER TABLE [dbo].[CollectionDetail] ADD  CONSTRAINT [DF__Collectio__Updat__6D0D32F4]  DEFAULT ('DataMigration') FOR [UpdatedBy]
GO


