﻿CREATE TABLE [dbo].[SigningNotificationFeed](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FeedDateTimeUtc] [datetime] NOT NULL,
	[PageNumber] INT NOT NULL,
	[SyndicationFeedId] [uniqueidentifier] NOT NULL,
	DateTimeUpdatedUtc datetime NOT NULL
 CONSTRAINT [PK_SigninFeedItem] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]