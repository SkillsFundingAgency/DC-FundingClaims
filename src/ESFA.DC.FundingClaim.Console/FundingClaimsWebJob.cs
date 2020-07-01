using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using ESFA.DC.FundingClaim.Console.Interfaces;
using ESFA.DC.FundingClaim.Console.Settings;
using ESFA.DC.FundingClaims.Data;
using ESFA.DC.FundingClaims.Message;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Queueing.Interface;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.FundingClaim.Console
{
    public sealed class FundingClaimsWebJob : IFundingClaimsWebJob<MessageFundingClaimsSubmission>
    {
        private readonly IQueueSubscriptionService<MessageFundingClaimsSubmission> _queueSubscriptionService;
        private ConnectionStrings _connectionStingDC;
        private IFundingClaimsDataContext _fundingClaimsDataContextDCT;
        private ILogger _logger;

        public FundingClaimsWebJob(
            IQueueSubscriptionService<MessageFundingClaimsSubmission> queueSubscriptionService,
            IFundingClaimsDataContext fundingClaimsDataContextDCT,
            ConnectionStrings connectionStingDC,
            ILogger logger)
        {
            _queueSubscriptionService = queueSubscriptionService;
            _fundingClaimsDataContextDCT = fundingClaimsDataContextDCT;
            _connectionStingDC = connectionStingDC;
            _logger = logger;
            _logger.LogInfo($"consructor of funding claim web job called.");
        }

        public void Subscribe()
        {
            _logger.LogInfo($"subscribe method of funding claim web job called.");
            _queueSubscriptionService.Subscribe(ProcessFundingClaimSubmissionAsync, CancellationToken.None);
        }

        private async Task<IQueueCallbackResult> ProcessFundingClaimSubmissionAsync(MessageFundingClaimsSubmission messageFundingClaimsSubmission, IDictionary<string, object> messageProperties, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInfo($"Starting funding claim web job.");

                if (!(await _fundingClaimsDataContextDCT.FundingClaimsSubmissionFile.AnyAsync(x => x.SubmissionId == messageFundingClaimsSubmission.SubmissionId)))
                {
                    throw new Exception($"submissionId does not exist in the database: submissionId :{messageFundingClaimsSubmission.SubmissionId}");
                }

                try
                {
                    _logger.LogInfo($"submissionId : {messageFundingClaimsSubmission.SubmissionId} retrieved from service bus, starting to fectch data from DCT database.");

                    var fundingClaimsSubmissionFile = await _fundingClaimsDataContextDCT.FundingClaimsSubmissionFile.Where(x => x.SubmissionId == messageFundingClaimsSubmission.SubmissionId).FirstAsync();

                    var fundingClaimsSubmissionValues = await _fundingClaimsDataContextDCT.FundingClaimsSubmissionValues.Where(x => x.SubmissionId == messageFundingClaimsSubmission.SubmissionId).ToListAsync();

                    var fundingClaimMaxContractValues = await _fundingClaimsDataContextDCT.FundingClaimMaxContractValues.Where(x => x.SubmissionId == messageFundingClaimsSubmission.SubmissionId).ToListAsync();

                    using (var connection = new SqlConnection(_connectionStingDC.DCFTFundingClaimsData))
                    {
                        await connection.OpenAsync();

                        using (var transaction = connection.BeginTransaction())
                        {
                            _logger.LogInfo($"Starting to copy data from DCT to DC database for submissionId : {messageFundingClaimsSubmission.SubmissionId}");

                            await connection.ExecuteAsync(
                                "INSERT INTO [dbo].[FundingClaimsSubmissionFile] ([SubmissionId] ,[UKPRN] ,[CollectionPeriod] ,[ProviderName] ,[UpdatedOn] ,[UpdatedBy] ,[IsHeProvider] ,[Declaration] ,[AsbMaximumContractValue] ,[DlsMaximumContractValue] ,[Allb24PlsMaximumContractValue] ,[SubmissionType] ,[Version] ,[IsSigned] ,[periodTypeCode] ,[period] ,[organisationIdentifier] ,[ClContractValue] ,[SignedOn])" +
                                "values(@SubmissionId, @UKPRN, @CollectionPeriod, @ProviderName, @UpdatedOn, @UpdatedBy, @IsHeProvider, @Declaration, @AsbMaximumContractValue, @DlsMaximumContractValue, @Allb24PlsMaximumContractValue, @SubmissionType, @Version, @IsSigned, @periodTypeCode, @period, @organisationIdentifier, @ClContractValue, @SignedOn)",
                                fundingClaimsSubmissionFile,
                                transaction);

                            await connection.ExecuteAsync(
                                    "INSERT INTO[dbo].[FundingClaimsSubmissionValues]" +
                                    "([SubmissionId],[CollectionPeriod],[DeliverableCode],[DeliverableDescription],[DeliveryToDate],[ForecastedDelivery],[ExceptionalAdjustments],[TotalDelivery],[FundingStreamPeriodCode],[contractAllocationNumber],[StudentNumbers])" +
                                    "VALUES(@SubmissionId  , @CollectionPeriod  , @DeliverableCode  , @DeliverableDescription  , @DeliveryToDate  , @ForecastedDelivery  , @ExceptionalAdjustments  , @TotalDelivery  , @FundingStreamPeriodCode  , @contractAllocationNumber  , @StudentNumbers )",
                                    fundingClaimsSubmissionValues,
                                    transaction);

                            await connection.ExecuteAsync(
                                    "INSERT INTO [dbo].[FundingClaimMaxContractValues] ([SubmissionId] ,[FundingStreamPeriodCode] ,[MaximumContractValue])" +
                                        "VALUES(@SubmissionId, @FundingStreamPeriodCode, @MaximumContractValue)",
                                    fundingClaimMaxContractValues,
                                    transaction);

                            transaction.Commit();

                            _logger.LogInfo($"Data copied across to DC database for submissionId : {messageFundingClaimsSubmission.SubmissionId}");
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError($"error occured while copying data to DC database for submissionId : {messageFundingClaimsSubmission.SubmissionId}", e);
                }

                _logger.LogInfo($"Finishing funding claim web job.");
                return new QueueCallbackResult(true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to process funding claim web job", ex);
                return new QueueCallbackResult(false, ex);
            }
        }
    }
}
