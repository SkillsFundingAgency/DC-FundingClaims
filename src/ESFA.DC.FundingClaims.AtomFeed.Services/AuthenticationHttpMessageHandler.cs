using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FundingClaims.AtomFeed.Services.Config;
using ESFA.DC.FundingClaims.AtomFeed.Services.Interfaces;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Polly;

namespace ESFA.DC.FundingClaims.AtomFeed.Services
{
    public class AuthenticationHttpMessageHandler : DelegatingHandler
    {
        private readonly AtomFeedSettings _atomFeedSettings;

        public AuthenticationHttpMessageHandler(AtomFeedSettings atomFeedSettings)
        {
            _atomFeedSettings = atomFeedSettings;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token =  await ProvideAsync();

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/atom+xml"));


            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }

        public async Task<string> ProvideAsync()
        {
            var policy = Policy
                .Handle<AdalException>(ex => ex.ErrorCode == "temporarily_unavailable")
                .WaitAndRetryAsync(3, a => TimeSpan.FromSeconds(3));

            return await policy.ExecuteAsync(async () => await AcquireTokenAsync());
        }

        private async Task<string> AcquireTokenAsync()
        {
            var authContext = new AuthenticationContext(_atomFeedSettings.Authority);
            var clientCredential = new ClientCredential(_atomFeedSettings.ClientId, _atomFeedSettings.AppKey);

            var authResult = await authContext.AcquireTokenAsync(_atomFeedSettings.ResourceId, clientCredential);

            if (authResult == null)
            {
                throw new AuthenticationException("Could not authenticate with the OAUTH2 claims provider after several attempts");
            }

            return authResult.AccessToken;
        }
    }
}
