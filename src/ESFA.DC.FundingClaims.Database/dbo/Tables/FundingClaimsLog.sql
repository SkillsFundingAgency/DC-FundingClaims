CREATE TABLE [dbo].[FundingClaimsLog](
	[Id] [int] NOT NULL,
	CollectionName nvarchar(50) NOT NULL,
	[UserEmailAddress] [nvarchar](320) NOT NULL,
	[UpdatedDateTimeUtc] [datetime] NOT NULL,
 CONSTRAINT [PK_DraftFundingClaimsSupportingData] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

