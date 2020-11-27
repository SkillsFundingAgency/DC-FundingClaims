CREATE TABLE [Reference].[FundingClaimsProviderReferenceData](
	[UKPRN] [int] NOT NULL,
	[EditAccess] [bit] NOT NULL,
	[AEBC-CLAllocation] [decimal](10, 2) NULL,
 CONSTRAINT [PK_FundingClaimsProviderReferenceData] PRIMARY KEY CLUSTERED 
(
	[UKPRN] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
