CREATE TABLE [dbo].[FundingClaimDetails] (
    [DataCollectionKey]   NVARCHAR (50) NOT NULL,
    [SubmissionOpenDate]  DATETIME      NOT NULL,
    [SubmissionCloseDate] DATETIME      NOT NULL,
    [SignatureCloseDate]  DATETIME      NOT NULL,
    [RequiresSignature]   BIT           NOT NULL,
    CONSTRAINT [PK_FundingClaimDetails] PRIMARY KEY CLUSTERED ([DataCollectionKey] ASC)
);

