using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.FundingClaims.Services.Interfaces;
using ESFA.DC.FundingClaims.Model;
using ESFA.DC.FundingClaims.ReferenceData.Services.Interfaces;
using ESFA.DC.Logging.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.FundingClaims.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/fc")]
    public class FundingClaimsController : ControllerBase
    {
        private readonly IFundingClaimsService _fundingClaimsService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILogger _logger;
        private readonly IFundingClaimsReferenceDataService _fundingClaimsReferenceDataService;
        private readonly ICollectionReferenceDataService _collectionReferenceDataService;

        public FundingClaimsController(
            IFundingClaimsService fundingClaimsService,
            IDateTimeProvider dateTimeProvider,
            ILogger logger,
            IFundingClaimsReferenceDataService fundingClaimsReferenceDataService,
            ICollectionReferenceDataService collectionReferenceDataService)
        {
            _fundingClaimsService = fundingClaimsService;
            _dateTimeProvider = dateTimeProvider;
            _logger = logger;
            _fundingClaimsReferenceDataService = fundingClaimsReferenceDataService;
            _collectionReferenceDataService = collectionReferenceDataService;
        }

        [HttpGet("provider-reference/{ukprn}/{collectionYear}")]
        public async Task<IActionResult> GetProviderReferenceData(CancellationToken cancellationToken, long ukprn, int collectionYear)
        {
            if (ukprn == 0)
            {
                return BadRequest();
            }

            try
            {
                ProviderReferenceData data;
                if (collectionYear == 1819)
                {
                    data = await _fundingClaimsReferenceDataService.GetProviderReferenceDataAsync(cancellationToken, ukprn);
                }
                else
                {
                    data = await _fundingClaimsReferenceDataService.GetProviderReferenceDataAsync(cancellationToken, ukprn, collectionYear);
                }

                _logger.LogInfo($"Returning reference data for ukprn : {ukprn}");
                return Ok(data);
            }
            catch (Exception e)
            {
                _logger.LogError($"error occured to get reference data for ukprn : {ukprn}", e);
                return BadRequest();
            }
        }

        [HttpGet("16To19/{ukprn}")]
        public async Task<IActionResult> Get1619FundingClaimDetails(CancellationToken cancellationToken, long ukprn)
        {
            if (ukprn == 0)
            {
                return BadRequest();
            }

            try
            {
                var result = await _fundingClaimsReferenceDataService.Get1619FundingClaimDetailsAsync(cancellationToken, ukprn);
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError($"error occured to Get1619FundingClaimDetails data for ukprn : {ukprn}", e);
                return BadRequest();
            }
        }

        [HttpGet("contract-allocations/{ukprn}/{collectionYear}")]
        public async Task<IActionResult> GetContractAllocations(CancellationToken cancellationToken, long ukprn, int collectionYear)
        {
            if (ukprn == 0)
            {
                return BadRequest();
            }

            try
            {
                var items = await _fundingClaimsReferenceDataService.GetContractAllocationsAsync(cancellationToken, ukprn, collectionYear);
                return Ok(items);
            }
            catch (Exception e)
            {
                _logger.LogError($"error occured to GetContractAllocations data for ukprn : {ukprn} and period : {collectionYear}", e);
                return BadRequest();
            }
        }

        [HttpGet("delivery-to-date/{ukprn}/{periodFrom}/{periodTo}/{collectionReturnCode}/{collectionYear}")]
        public async Task<IActionResult> GetDeliveryToDateValues(CancellationToken cancellationToken, long ukprn, int periodFrom, int periodTo, string collectionReturnCode, int collectionYear)
        {
            if (ukprn == 0)
            {
                return BadRequest();
            }

            try
            {
                IEnumerable<SummarisedActualDeliveryToDate> result = await _fundingClaimsReferenceDataService
                    .GetDeliveryToDateValuesAsync(cancellationToken, ukprn, periodFrom, periodTo, collectionReturnCode, collectionYear);

                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError($"error occured to GetDeliveryToDateValues data for ukprn : {ukprn}  period from: {periodFrom}, period to : {periodTo}, collection return code :{collectionReturnCode} ", e);
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("draft")]
        public async Task<IActionResult> SaveDraftValuesAsync(CancellationToken cancellationToken, [FromBody] FundingClaimsData draftFundingClaims)
        {
            _logger.LogInfo($"Save draft received for ukprn :{draftFundingClaims?.Ukprn}, items count : {draftFundingClaims?.FundingClaimsDataItems?.Count}");

            if (draftFundingClaims?.Ukprn == 0)
            {
                return BadRequest();
            }

            if (draftFundingClaims == null || draftFundingClaims.FundingClaimsDataItems == null || !draftFundingClaims.FundingClaimsDataItems.Any())
            {
                _logger.LogError($"Save draft received with no data for ukprn :{draftFundingClaims?.Ukprn}");
                return BadRequest("no data submitted");
            }

            try
            {
                await _fundingClaimsService.SaveDraftAsync(cancellationToken, draftFundingClaims);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError($"error occured to SaveDraftValuesAsync data for ukprn : {draftFundingClaims?.Ukprn}  collection period : {draftFundingClaims?.CollectionCode} ", e);
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("draft/{collectionCode}/{ukprn}")]
        public async Task<IActionResult> GetDraftValuesAsync(CancellationToken cancellationToken, string collectionCode, long ukprn)
        {
            _logger.LogInfo($"Get draft received for ukprn :{ukprn}, collection period : {collectionCode}");

            if (ukprn == 0)
            {
                return BadRequest();
            }

            try
            {
                var data = await _fundingClaimsService.GetDraftAsync(cancellationToken, collectionCode, ukprn);
                return Ok(data);
            }
            catch (Exception e)
            {
                _logger.LogError($"error occured to GetDraftValuesAsync data for ukprn : {ukprn}  collection period : {collectionCode} ", e);
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("submit/{collectionCode}/{academicYear}/{ukprn}")]
        public async Task<IActionResult> ConvertToSubmission(CancellationToken cancellationToken, string collectionCode, int academicYear, long ukprn)
        {
            _logger.LogInfo($"ConvertToSubmission received for ukprn :{ukprn}, collection period : {collectionCode}");

            if (ukprn == 0)
            {
                return BadRequest();
            }

            try
            {
                var latestSubmissionVersion = await _fundingClaimsService.GetLatestSubmissionVersionAsync(cancellationToken, ukprn);

                var data = await _fundingClaimsService.ConvertToSubmissionAsync(cancellationToken, ukprn, latestSubmissionVersion, collectionCode, academicYear);
                return Ok(data);
            }
            catch (Exception e)
            {
                _logger.LogError($"error occured to ConvertToSubmission data for ukprn : {ukprn}  collection period : {collectionCode} ", e);
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("cof-removal/{ukprn}")]
        public async Task<IActionResult> GetCofRemovalValue(CancellationToken cancellationToken, long ukprn)
        {
            try
            {
                var result = await _fundingClaimsReferenceDataService.GetOrganisationDetailsAsync(cancellationToken,ukprn);
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError($"error occured to GetCofRemovalValue data for ukprn : {ukprn}", e);
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("submissions/{ukprn}")]
        public async Task<IActionResult> GetSubmissionHistoryAsync(CancellationToken cancellationToken, long ukprn)
        {
            _logger.LogInfo($"GetSubmissionHistoryAsync received for ukprn :{ukprn}");

            if (ukprn == 0)
            {
                return BadRequest();
            }

            try
            {
                var data = await _fundingClaimsService.GetSubmissionHistoryAsync(cancellationToken, ukprn);
                return Ok(data);
            }
            catch (Exception e)
            {
                _logger.LogError($"error occured to GetSubmissionHistoryAsync data for ukprn : {ukprn}", e);
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("submission-detail/{ukprn}/{submissionId}")]
        public async Task<IActionResult> GetSubmissionDetails(CancellationToken cancellationToken, long ukprn, string submissionId)
        {
            _logger.LogInfo($"GetSubmissionDetails received for ukprn :{ukprn}, submissionId : {submissionId}");

            if (ukprn == 0 || string.IsNullOrEmpty(submissionId))
            {
                return BadRequest();
            }

            try
            {
                var data = await _fundingClaimsService.GetSubmissionAsync(cancellationToken, new Guid(submissionId), ukprn);
                return Ok(data);
            }
            catch (Exception e)
            {
                _logger.LogError($"error occured to GetSubmissionDetails data for ukprn : {ukprn}, submission Id : {submissionId}", e);
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("submission-contract-values/{ukprn}/{submissionId}")]
        public async Task<IActionResult> GetSubmissionContractValues(CancellationToken cancellationToken, long ukprn, string submissionId)
        {
            _logger.LogInfo($"GetSubmissionContractValues received for ukprn :{ukprn}, submissionId : {submissionId}");

            if (ukprn == 0 || string.IsNullOrEmpty(submissionId))
            {
                return BadRequest();
            }

            try
            {
                var data = await _fundingClaimsService.GetSubmittedMaxContractValuesAsync(cancellationToken, ukprn, new Guid(submissionId));
                return Ok(data);
            }
            catch (Exception e)
            {
                _logger.LogError($"error occured to GetSubmissionContractValues  data for ukprn : {ukprn}, submission Id : {submissionId}", e);
                return BadRequest();
            }
        }

        [HttpGet("collection/{dateTimeUtc?}")]
        public async Task<FundingClaimsCollection> GetFundingClaimsCollection(CancellationToken cancellationToken, DateTime? dateTimeUtc = null)
        {
            dateTimeUtc = dateTimeUtc ?? _dateTimeProvider.GetNowUtc();
            return await _collectionReferenceDataService.GetFundingClaimsCollectionAsync(cancellationToken, dateTimeUtc);
        }

        [HttpGet("collection/code/{collectionCode}")]
        public async Task<FundingClaimsCollection> GetFundingClaimsCollection(CancellationToken cancellationToken, string collectionCode)
        {
            return await _collectionReferenceDataService.GetFundingClaimsCollectionAsync(cancellationToken, collectionCode);
        }
    }
}
