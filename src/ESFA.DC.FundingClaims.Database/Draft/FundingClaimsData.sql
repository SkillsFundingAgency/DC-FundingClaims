CREATE TABLE [Draft].[FundingClaimsData] (
    [Id]                       BIGINT           IDENTITY (1, 1) NOT NULL,
    [CollectionName]         NVARCHAR (50)    NOT NULL,
    [DeliverableCode]          INT              NOT NULL,
    [DeliverableDescription]   NVARCHAR (1000)  NOT NULL,
    [DeliveryToDate]           DECIMAL (10, 2)  NULL,
    [ForecastedDelivery]       DECIMAL (10, 2)  NULL,
    [ExceptionalAdjustments]   DECIMAL (10, 2)  NULL,
    [TotalDelivery]            DECIMAL (12, 2)  NULL,
    [FundingStreamPeriodCode]  NVARCHAR (50)    NULL,
    [contractAllocationNumber] NVARCHAR (100)   NULL,
    [StudentNumbers]           INT              NULL,
	[Ukprn]		BIGINT NOT NULL,
    CONSTRAINT [PK_DraftFundingClaimsData] PRIMARY KEY CLUSTERED ([Id] ASC)
);

