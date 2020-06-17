using ESFA.DC.ReferenceData.FCS.Service.Config.Interface;

namespace ESFA.DC.ReferenceData.FCS.Service.Config
{
    public class FcsServiceConfiguration : IFcsServiceConfiguration
    {
        public string FeedUri { get; set; }

        public string Authority { get; set; }

        public string ResourceId { get; set; }

        public string ClientId { get; set; }

        public string AppKey { get; set; }

        public string FcsConnectionString { get; set; }
    }
}
