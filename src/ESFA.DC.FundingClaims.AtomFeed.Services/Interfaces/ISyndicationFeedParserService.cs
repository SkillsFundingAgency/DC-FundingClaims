using System;
using System.ServiceModel.Syndication;

namespace ESFA.DC.FundingClaims.AtomFeed.Services.Interfaces
{
    public interface ISyndicationFeedParserService<T>
    {
        string PreviousArchiveLink(SyndicationFeed syndicationFeed);

        string CurrentArchiveLink(SyndicationFeed syndicationFeed);

        string NextArchiveLink(SyndicationFeed syndicationFeed);

        (Guid syndicationItemId, T model) RetrieveDataFromSyndicationItem(SyndicationItem syndicationItem);
    }
}
