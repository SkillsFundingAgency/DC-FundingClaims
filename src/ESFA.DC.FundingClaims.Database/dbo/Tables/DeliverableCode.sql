CREATE TABLE [dbo].[DeliverableCode](
	[DeliverableCodeId] [int] NOT NULL,
	[Description] [nvarchar](1000) NOT NULL,
	[CollectionName] NVARCHAR(50) NULL, 
    CONSTRAINT [PK_DeliverableCode] PRIMARY KEY CLUSTERED 
(
	[DeliverableCodeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

