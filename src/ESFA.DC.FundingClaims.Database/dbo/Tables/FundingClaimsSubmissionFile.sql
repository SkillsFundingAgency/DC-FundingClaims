
CREATE TABLE [dbo].[FundingClaimsSubmissionFile](
	[SubmissionId] [uniqueidentifier] NOT NULL,
	[UKPRN] [nvarchar](10) NOT NULL,
	[CollectionPeriod] [nvarchar](50) NOT NULL,
	[ProviderName] [nvarchar](250) NOT NULL,
	[UpdatedOn] [datetime] NOT NULL,
	[UpdatedBy] [nvarchar](250) NOT NULL,
	[IsHeProvider] [bit] NOT NULL,
	[Declaration] [bit] NOT NULL,
	[AsbMaximumContractValue] [decimal](16, 2) NULL,
	[DlsMaximumContractValue] [decimal](16, 2) NULL,
	[Allb24PlsMaximumContractValue] [decimal](16, 2) NULL,
	[SubmissionType] [int] NOT NULL,
	[Version] [int] NOT NULL,
	[IsSigned] [bit] NOT NULL,
	[periodTypeCode] [nvarchar](100) NULL,
	[period] [nvarchar](100) NULL,
	[organisationIdentifier] [nvarchar](100) NULL,
	[ClContractValue] [decimal](16, 2) NULL,
	[SignedOn] [datetime] NULL,
 CONSTRAINT [PK_SubmissionID] PRIMARY KEY CLUSTERED 
(
	[SubmissionId] ASC,
	[CollectionPeriod] ASC,
	[SubmissionType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE NONCLUSTERED INDEX [IX_Ukprn_CollectionPeriod] ON [dbo].[FundingClaimsSubmissionFile]
(
	[UKPRN] ASC, [CollectionPeriod] ASC
) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

ALTER TABLE [dbo].[FundingClaimsSubmissionFile] ADD  CONSTRAINT [DF_FundingClaimsSubmissionFile_Version]  DEFAULT ((0)) FOR [Version]
GO

ALTER TABLE [dbo].[FundingClaimsSubmissionFile] ADD  CONSTRAINT [DF_FundingClaimsSubmissionFile_IsSigned]  DEFAULT ((0)) FOR [IsSigned]
GO

ALTER TABLE [dbo].[FundingClaimsSubmissionFile]  WITH CHECK ADD  CONSTRAINT [FK_SubmissionType] FOREIGN KEY([SubmissionType])
REFERENCES [Static].[SubmissionTypes] ([Id])
GO

ALTER TABLE [dbo].[FundingClaimsSubmissionFile] CHECK CONSTRAINT [FK_SubmissionType]
GO


