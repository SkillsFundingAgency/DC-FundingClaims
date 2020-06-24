using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ESFA.DC.FundingClaims.Signing.Models
{
    [Serializable]
    [XmlType(AnonymousType = true)]
    [XmlRoot(ElementName = "FundingClaim", IsNullable = false)]
    public class FundingClaimsFeedItem
    {
        [XmlElement("FundingClaimId")]
        public string FundingClaimId { get; set; }
        [XmlElement("HasBeenSigned")]
        public bool HasBeenSigned { get; set; }
    }
}
