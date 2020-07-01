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
        private readonly ILogger _logger;
        private readonly DelegatingHandler _delegatingHandler;

        public SyndicationFeedService(ILogger logger, DelegatingHandler delegatingHandler)
        {
            _logger = logger;
            _delegatingHandler = delegatingHandler;
        }

        public async Task<SyndicationFeed> LoadSyndicationFeedFromUriAsync(string uri, CancellationToken cancellationToken)
        {

            var httpClient = new HttpClient(_delegatingHandler);
            var response = await httpClient.GetAsync(uri, cancellationToken);

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
