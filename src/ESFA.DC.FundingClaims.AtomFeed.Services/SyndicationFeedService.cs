using System;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using ESFA.DC.FundingClaims.AtomFeed.Services.Config;
using ESFA.DC.FundingClaims.AtomFeed.Services.Interfaces;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.FundingClaims.AtomFeed.Services
{
    public class SyndicationFeedService : ISyndicationFeedService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger _logger;
        private readonly AtomFeedSettings _atomFeedSettings;

        //public SyndicationFeedService(IHttpClientFactory httpClientFactory, ILogger logger, AtomFeedSettings atomFeedSettings)
        //{
        //    _httpClientFactory = httpClientFactory;
        //    _logger = logger;
        //    _atomFeedSettings = atomFeedSettings;
        //}
        public SyndicationFeedService(ILogger logger, AtomFeedSettings atomFeedSettings)
        {
            _logger = logger;
            _atomFeedSettings = atomFeedSettings;
        }

        public async Task<SyndicationFeed> LoadSyndicationFeedFromUriAsync(string uri, CancellationToken cancellationToken)
        {
            HttpResponseMessage response;

            //TODO: review client and client factory ect
            var httpClient = new HttpClient(new AuthenticationHttpMessageHandler(_atomFeedSettings));

            //var httpClient = _httpClientFactory.CreateClient();
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
