using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.FundingClaims.EmailNotification.Services
{
    public interface INotifierConfig
    {
        string ApiKey { get; }
    }
}
