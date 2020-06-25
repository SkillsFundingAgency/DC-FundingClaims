﻿CREATE TABLE [dbo].[SigningNotificationFeed](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FeedDateTime] [datetime] NOT NULL,
	[LatestFeedUri] [varchar](max) NOT NULL,
	[SyndicationFeedId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_SigninFeedItem] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]