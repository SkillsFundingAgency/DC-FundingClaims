using System.ServiceModel.Syndication;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.FundingClaims.AtomFeed.Services.Interfaces
{
    public interface ISyndicationFeedService
    {
        Task<SyndicationFeed> LoadSyndicationFeedFromUriAsync(string uri, CancellationToken cancellationToken);
    }
}
