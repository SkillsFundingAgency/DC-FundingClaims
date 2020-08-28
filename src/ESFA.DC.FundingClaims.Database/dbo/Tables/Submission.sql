CREATE TABLE [dbo].[Submission](
	[SubmissionId] [uniqueidentifier] NOT NULL,
	[UKPRN] [bigint] NOT NULL,
	[CollectionName] [nvarchar](50) NOT NULL,
	[CollectionYear] [int] NOT NULL,
	[Declaration] [bit] NULL,
	[Version] [int] NOT NULL DEFAULT 0,
	[IsSigned] [bit] NOT NULL DEFAULT 0,
	[OrganisationIdentifier] [nvarchar](100) NULL,
	[SubmittedBy] [nvarchar](250) NULL,
	[IsSubmitted] BIT NOT NULL DEFAULT 0, 
	[SubmittedDateTimeUtc] [datetime] NULL,
	[SignedOnDateTimeUtc] [datetime] NULL,
	[CreatedBy] NVARCHAR(250) NOT NULL, 
    [CreatedDateTimeUtc] datetime NOT NULL, 
    
    CONSTRAINT [PK_FundingClaimsSubmissionFile_SubmissionID] PRIMARY KEY CLUSTERED 
(
	[SubmissionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
