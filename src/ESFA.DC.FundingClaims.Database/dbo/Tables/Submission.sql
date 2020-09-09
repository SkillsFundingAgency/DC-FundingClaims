CREATE TABLE [dbo].[Submission](
	[SubmissionId] [uniqueidentifier] NOT NULL,
	[UKPRN] [bigint] NOT NULL,
	[CollectionId] [int] NOT NULL,
	[Declaration] [bit] NULL,
	[Version] [int] NOT NULL,
	[IsSigned] [bit] NOT NULL,
	[OrganisationIdentifier] [nvarchar](100) NULL,
	[SubmittedBy] [nvarchar](250) NULL,
	[IsSubmitted] [bit] NOT NULL,
	[SubmittedDateTimeUtc] [datetime] NULL,
	[SignedOnDateTimeUtc] [datetime] NULL,
	[CreatedBy] [nvarchar](250) NOT NULL,
	[CreatedDateTimeUtc] [datetime] NOT NULL,
 CONSTRAINT [PK_FundingClaimsSubmissionFile_SubmissionID] PRIMARY KEY CLUSTERED 
(
	[SubmissionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Submission] ADD  CONSTRAINT [DF__Submissio__Versi__4AB81AF0]  DEFAULT ((0)) FOR [Version]
GO

ALTER TABLE [dbo].[Submission] ADD  CONSTRAINT [DF__Submissio__IsSig__4BAC3F29]  DEFAULT ((0)) FOR [IsSigned]
GO

ALTER TABLE [dbo].[Submission] ADD  CONSTRAINT [DF__Submissio__IsSub__4CA06362]  DEFAULT ((0)) FOR [IsSubmitted]
GO

ALTER TABLE [dbo].[Submission]  WITH CHECK ADD  CONSTRAINT [FK_Submission_CollectionDetail] FOREIGN KEY([CollectionId])
REFERENCES [dbo].[CollectionDetail] ([CollectionId])
GO

ALTER TABLE [dbo].[Submission] CHECK CONSTRAINT [FK_Submission_CollectionDetail]
GO
