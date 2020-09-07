CREATE TABLE [dbo].[SubmissionValue](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[SubmissionId] [uniqueidentifier] NOT NULL,
	[DeliverableCodeId] [int] NOT NULL,
	[DeliveryToDate] [decimal](16, 2) NOT NULL,
	[ForecastedDelivery] [decimal](16, 2) NOT NULL,
	[ExceptionalAdjustments] [decimal](16, 2) NOT NULL,
	[TotalDelivery] [decimal](16, 2) NOT NULL,
	[StudentNumbers] [int] NOT NULL,
	[FundingStreamPeriodCode] [varchar](50) NOT NULL,
	[ContractAllocationNumber] [varchar](100) NOT NULL,
 CONSTRAINT [PK_SubmissionValue] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[SubmissionValue] ADD  CONSTRAINT [DF__FundingCl__Forec__48CFD27E]  DEFAULT ((0)) FOR [ForecastedDelivery]
GO

ALTER TABLE [dbo].[SubmissionValue] ADD  CONSTRAINT [DF__FundingCl__Excep__49C3F6B7]  DEFAULT ((0)) FOR [ExceptionalAdjustments]
GO

ALTER TABLE [dbo].[SubmissionValue] ADD  CONSTRAINT [DF__FundingCl__Total__4AB81AF0]  DEFAULT ((0)) FOR [TotalDelivery]
GO

ALTER TABLE [dbo].[SubmissionValue] ADD  CONSTRAINT [DF__FundingCl__Stude__4BAC3F29]  DEFAULT ((0)) FOR [StudentNumbers]
GO

ALTER TABLE [dbo].[SubmissionValue]  WITH CHECK ADD  CONSTRAINT [FK_SubmissionValue_FundingStreamPeriodDeliverableCode] FOREIGN KEY([DeliverableCodeId])
REFERENCES [dbo].[FundingStreamPeriodDeliverableCode] (Id)
GO

ALTER TABLE [dbo].[SubmissionValue] CHECK CONSTRAINT [FK_SubmissionValue_FundingStreamPeriodDeliverableCode]
GO

ALTER TABLE [dbo].[SubmissionValue]  WITH NOCHECK ADD  CONSTRAINT [FK_SubmissionValue_Submission] FOREIGN KEY([SubmissionId])
REFERENCES [dbo].[Submission] ([SubmissionId])
GO

ALTER TABLE [dbo].[SubmissionValue] CHECK CONSTRAINT [FK_SubmissionValue_Submission]
GO


