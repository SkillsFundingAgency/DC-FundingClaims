using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
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

        public int CurrentPageNumber(SyndicationFeed syndicationFeed)
        {
            var previousLink =  RetrieveLinkForRelationshipType(syndicationFeed, PreviousArchive);
            if (string.IsNullOrEmpty(previousLink))
            {
                return 0;
            }

            var lastIndex = previousLink.LastIndexOf('/');
            int.TryParse(previousLink.Substring(lastIndex + 1), out var pageNumber);

            return pageNumber +1;
        }

        public string PreviousArchiveLink(SyndicationFeed syndicationFeed)
        {
            return RetrieveLinkForRelationshipType(syndicationFeed, PreviousArchive);
        }

        public string NextArchiveLink(SyndicationFeed syndicationFeed)
        {
            return RetrieveLinkForRelationshipType(syndicationFeed, NextArchive);
        }

        public T RetrieveDataFromSyndicationItem(SyndicationItem syndicationItem)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(stringWriter))
                {
                    syndicationItem.Content.WriteTo(xmlWriter, "temp", "temp");
                }

                var fundingClaimFeedItem = XDocument.Parse(stringWriter.ToString()).Elements().Descendants();

                using (var memoryStream = new MemoryStream())
                {
                    using (var xmlWriter = XmlWriter.Create(memoryStream))
                    {
                        fundingClaimFeedItem.First().WriteTo(xmlWriter);
                    }

                    memoryStream.Seek(0, SeekOrigin.Begin);

                    var model = _xmlserializationService.Deserialize<T>(memoryStream);
                    
                    return model;
                    
                }
            }
        }

        private string RetrieveLinkForRelationshipType(SyndicationFeed syndicationFeed, string relationshipType)
        {
            return syndicationFeed.Links.FirstOrDefault(l => l.RelationshipType == relationshipType)?.Uri?.ToString();
        }
    }
}
