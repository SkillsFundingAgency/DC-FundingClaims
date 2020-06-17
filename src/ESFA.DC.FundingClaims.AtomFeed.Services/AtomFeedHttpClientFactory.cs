//using System.Net.Http;
//using System.Net.Http.Headers;
//using ESFA.DC.FundingClaims.AtomFeed.Services.Interfaces;
//using Microsoft.IdentityModel.Clients.ActiveDirectory;

//namespace ESFA.DC.FundingClaims.AtomFeed.Services
//{
//    public class AtomFeedHttpClientFactory : Interfaces.AtomFeedHttpClientFactory
//    {
//        private const string MediaType = @"application/vnd.sfa.contract.v1+atom+xml";

//        private readonly IAccessTokenProvider _accessTokenProvider;
//        private readonly IHttpClientFactory _httpClientFactory;

//        public AtomFeedHttpClientFactory(IAccessTokenProvider accessTokenProvider, IHttpClientFactory httpClientFactory)
//        {
//            _accessTokenProvider = accessTokenProvider;
//            _httpClientFactory = httpClientFactory;
//        }

//        public HttpClient Create()
//        {
//            var client = _httpClientFactory.GetHttpClient();
//            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessTokenProvider.ProvideAsync().Result);
//            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaType));

//            return client;
//        }
//    }
//}
