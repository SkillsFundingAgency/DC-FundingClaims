CREATE TABLE [Draft].[FundingClaimsSupportingData]
(
	[Ukprn] BIGINT NOT NULL,
	[CollectionCode] NVARCHAR(50) NOT NULL,
	[UserEmailAddress] NVARCHAR(320) NOT NULL,
	[LastUpdatedDateTimeUtc] DATETIME NOT NULL DEFAULT GETDATE() 

	CONSTRAINT [PK_DraftFundingClaimsSupportingData] PRIMARY KEY CLUSTERED ([Ukprn], [CollectionCode] ASC)
)
