CREATE TABLE [dbo].[SubmissionContractDetail](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FundingStreamPeriodCode] [nvarchar](50) NOT NULL,
	[ContractValue] [decimal](16, 2) NOT NULL,
	[SubmissionId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_FundingClaimsContract] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[SubmissionContractDetail]  WITH CHECK ADD  CONSTRAINT [FK_SubmissionContractDetail_Submission] FOREIGN KEY([SubmissionId])
REFERENCES [dbo].[Submission] ([SubmissionId])
GO

ALTER TABLE [dbo].[SubmissionContractDetail] CHECK CONSTRAINT [FK_SubmissionContractDetail_Submission]
GO

