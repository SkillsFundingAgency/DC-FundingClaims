CREATE TABLE [dbo].[FundingClaimsSubmissionValues] (
    [Id]                       BIGINT           IDENTITY (1, 1) NOT NULL,
    [SubmissionId]             UNIQUEIDENTIFIER NOT NULL,
    [CollectionPeriod]         NVARCHAR (50)    NOT NULL,
    [DeliverableCode]          INT              NOT NULL,
    [DeliverableDescription]   NVARCHAR (1000)  NOT NULL,
    [DeliveryToDate]           DECIMAL (10, 2)  NULL DEFAULT 0,
    [ForecastedDelivery]       DECIMAL (10, 2)  NULL DEFAULT 0,
    [ExceptionalAdjustments]   DECIMAL (10, 2)  NULL DEFAULT 0,
    [TotalDelivery]            DECIMAL (12, 2)  NULL DEFAULT 0,
    [FundingStreamPeriodCode]  NVARCHAR (50)    NULL,
    [contractAllocationNumber] NVARCHAR (100)   NULL,
    [StudentNumbers]           INT              NULL DEFAULT 0,
    CONSTRAINT [PK_FundingClaimsSubmissionValues] PRIMARY KEY CLUSTERED ([Id] ASC)
);

