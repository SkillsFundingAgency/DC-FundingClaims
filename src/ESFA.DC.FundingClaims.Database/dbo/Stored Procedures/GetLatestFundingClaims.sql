-- =============================================
-- Author:		Name
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE GetLatestFundingClaims
	-- Add the parameters for the stored procedure here
	@sinceDateTime datetime,
	@requireSigntaure bit = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	Select * from 
		(Select *,
			ROW_NUMBER() OVER(PARTITION BY [Ukprn],[collectionPeriod],[submissionType] ORDER BY [Version] DESC) As rn
		from FundingClaimsSubmissionFile ) x
		Inner Join FundingClaimDetails fc on 
		x.[Period]+'-'+
						 (CAse when CollectionPeriod='FC01' then 'MidYear'
								when CollectionPeriod='FC02' then 'YearEnd'
								when CollectionPeriod='FC03' then 'Final'
							End)
						  = fc.DataCollectionKey

		Where x.UpdatedOn > @sinceDateTime
		And fc.RequiresSignature = @requireSigntaure
		And x.rn = 1
END