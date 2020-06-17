using System;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Xml.Linq;
using ESFA.DC.FundingClaims.AtomFeed.Services.Config;
using ESFA.DC.FundingClaims.AtomFeed.Services.Interfaces;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.FundingClaims.AtomFeed.Services
{
    public class SyndicationFeedParserService<T> : ISyndicationFeedParserService<T>
    {
        private const string PreviousArchive = "prev-archive";
        private const string CurrentArchive = "current";
        private const string NextArchive = "next-archive";
        private readonly IXmlSerializationService _xmlserializationService;
        private readonly AtomFeedSettings _atomFeedSettings;

        public SyndicationFeedParserService(IXmlSerializationService xmlSerializationService, AtomFeedSettings atomFeedSettings)
        {
            _xmlserializationService = xmlSerializationService;
            _atomFeedSettings = atomFeedSettings;
        }

        public string CurrentArchiveLink(SyndicationFeed syndicationFeed)
        {
            return RetrieveLinkForRelationshipType(syndicationFeed, CurrentArchive);
        }

        public string PreviousArchiveLink(SyndicationFeed syndicationFeed)
        {
            return RetrieveLinkForRelationshipType(syndicationFeed, PreviousArchive);
        }

        public string NextArchiveLink(SyndicationFeed syndicationFeed)
        {
            return RetrieveLinkForRelationshipType(syndicationFeed, NextArchive);
        }

        public(Guid syndicationItemId, T model) RetrieveContractFromSyndicationItem(SyndicationItem syndicationItem)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(stringWriter))
                {
                    syndicationItem.Content.WriteTo(xmlWriter, "temp", "temp");
                }

                var contract = XDocument.Parse(stringWriter.ToString()).Descendants().Where(x => x.Name.LocalName == _atomFeedSettings.ParentEntityName).ToList();

                using (var memoryStream = new MemoryStream())
                {
                    using (var xmlWriter = XmlWriter.Create(memoryStream))
                    {
                        contract.First().WriteTo(xmlWriter);
                    }

                    memoryStream.Seek(0, SeekOrigin.Begin);

                    var model = _xmlserializationService.Deserialize<T>(memoryStream);

                    return (Guid.Parse(syndicationItem.Id.Remove(0, 5)), model);
                }
            }
        }

        private string RetrieveLinkForRelationshipType(SyndicationFeed syndicationFeed, string relationshipType)
        {
            return syndicationFeed.Links.FirstOrDefault(l => l.RelationshipType == relationshipType)?.Uri?.ToString();
        }
    }
}
