CREATE TABLE [dbo].[FundingClaimsFieldRule] (
    [Id]                      INT            IDENTITY (1, 1) NOT NULL,
    [DataCollectionKey]           NVARCHAR(50)  NOT NULL,
    [FundingStreamPeriodCode] NVARCHAR (50) NOT NULL,
    [DeliverableCode]         INT  NOT NULL,
    [MappedColumnName]        NVARCHAR (100) NOT NULL,
    [IsVisible]               BIT            NOT NULL,
    [IsEditable]              BIT            NOT NULL,
    CONSTRAINT [PK_FundingClaimsFieldRule] PRIMARY KEY CLUSTERED ([Id] ASC)
);

