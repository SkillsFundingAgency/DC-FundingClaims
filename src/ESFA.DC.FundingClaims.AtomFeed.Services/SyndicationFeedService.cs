using System;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using ESFA.DC.FundingClaims.AtomFeed.Services.Interfaces;
using ESFA.DC.Logging.Interfaces;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace ESFA.DC.FundingClaims.AtomFeed.Services
{
    public class SyndicationFeedService : ISyndicationFeedService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger _logger;

        public SyndicationFeedService(IHttpClientFactory httpClientFactory, ILogger logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<SyndicationFeed> LoadSyndicationFeedFromUriAsync(string uri, CancellationToken cancellationToken)
        {
            HttpResponseMessage response;

            var httpClient = _httpClientFactory.GetHttpClient();
            response = await httpClient.GetAsync(uri, cancellationToken);

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception exception)
            {
                _logger.LogError($"Http Request for {uri} Failed", exception);
                throw;
            }

            using (var contentStream = await response.Content.ReadAsStreamAsync())
            {
                using (var xmlReader = new XmlTextReader(contentStream))
                {
                    try
                    {
                        return SyndicationFeed.Load(xmlReader);
                    }
                    catch (XmlException xmlException)
                    {
                        _logger.LogInfo(response.Content.ReadAsStringAsync().Result);
                        _logger.LogError($"Syndication Feed Load with Xml Exception {uri} Failed", xmlException);

                        throw;
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError($"Syndication Feed Load for {uri} Failed", exception);

                        throw;
                    }
                }
            }
        }
    }
}
