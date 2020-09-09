CREATE TABLE [dbo].[FundingStreamPeriodDeliverableCode](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DeliverableCode] [int] NOT NULL,
	[Description] [nvarchar](1000) NOT NULL,
	[FundingStreamPeriodCode] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_FundingStreamPeriodDeliverableCode] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[FundingStreamPeriodDeliverableCode]  WITH CHECK ADD  CONSTRAINT [CK_FundingStreamPeriodDeliverableCode_Column] CHECK  (((1)=(1)))
GO

ALTER TABLE [dbo].[FundingStreamPeriodDeliverableCode] CHECK CONSTRAINT [CK_FundingStreamPeriodDeliverableCode_Column]
GO


