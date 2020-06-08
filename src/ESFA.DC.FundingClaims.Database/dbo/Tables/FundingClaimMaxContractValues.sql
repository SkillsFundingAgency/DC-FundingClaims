CREATE TABLE [dbo].[FundingClaimMaxContractValues] (
    [Id]      BIGINT            IDENTITY (1, 1) NOT NULL,
	[SubmissionId]            UNIQUEIDENTIFIER NOT NULL,
    [FundingStreamPeriodCode] NVARCHAR (50)    NULL,
    [MaximumContractValue]    DECIMAL (16, 2)  NULL,
	CONSTRAINT [PK_FundingClaimMaxContractValues] PRIMARY KEY CLUSTERED ([Id] ASC)

);

