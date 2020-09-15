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
    [Route("api/v{version:apiVersion}/collection")]
    public class CollectionDetailController : ControllerBase
    {
        private readonly ICollectionReferenceDataService _collectionReferenceDataService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public CollectionDetailController(ICollectionReferenceDataService collectionReferenceDataService, IDateTimeProvider dateTimeProvider)
        {
            _collectionReferenceDataService = collectionReferenceDataService;
            _dateTimeProvider = dateTimeProvider;
        }

        [HttpGet("{dateTimeUtc?}/{isHelpDesk?}")]
        public async Task<FundingClaimsCollection> GetFundingClaimsCollection(CancellationToken cancellationToken, DateTime? dateTimeUtc = null, bool isHelpDesk = false)
        {
            dateTimeUtc ??= _dateTimeProvider.GetNowUtc();
            return await _collectionReferenceDataService.GetFundingClaimsCollectionAsync(cancellationToken, dateTimeUtc, isHelpDesk);
        }

        [HttpGet("name/{collectionName}")]
        public async Task<FundingClaimsCollection> GetFundingClaimsCollection(CancellationToken cancellationToken, string collectionName)
        {
            return await _collectionReferenceDataService.GetFundingClaimsCollectionAsync(cancellationToken, collectionName);
        }

        [HttpGet]
        public async Task<IActionResult> GetCollectionAsync(CancellationToken cancellationToken)
        {
            var fundingClaimsCollectionMetaData = await _collectionReferenceDataService.GetAllFundingClaimsCollectionsAsync(cancellationToken);

            if (fundingClaimsCollectionMetaData.Any())
            {
                return Ok(fundingClaimsCollectionMetaData);
            }
            else
            {
                return NoContent();
            }
        }

        [HttpGet("lastupdate")]
        public async Task<IActionResult> GetFundingClaimsCollectionMetaDataLastUpdateAsync(CancellationToken cancellationToken)
        {
            var data = await _collectionReferenceDataService.GetLastUpdatedCollectionAsync(cancellationToken);

            if (data != null)
            {
                return Ok(data);
            }
            else
            {
                return NoContent();
            }
        }

        [HttpGet("collectionYear/{collectionYear}")]
        public async Task<IActionResult> GetFundingClaimsCollectionMetaDataAsync(CancellationToken cancellationToken, int collectionYear)
        {
            var data = await _collectionReferenceDataService.GetCollectionsAsync(cancellationToken, collectionYear);

            if (data.Any())
            {
                return Ok(data);
            }
            else
            {
                return NoContent();
            }
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateAsync(CancellationToken cancellationToken, [FromBody] FundingClaimsCollection dto)
        {
            var isSuccess = await _collectionReferenceDataService.UpdateCollection(cancellationToken, dto);

            if (isSuccess)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
