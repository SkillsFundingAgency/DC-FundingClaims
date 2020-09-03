CREATE TABLE [dbo].[DeliverableCode](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DeliverableCodeId] [int] NOT NULL,
	[Description] [nvarchar](1000) NOT NULL,
	[FundingStreamPeriodCode] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_DeliverableCode] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
