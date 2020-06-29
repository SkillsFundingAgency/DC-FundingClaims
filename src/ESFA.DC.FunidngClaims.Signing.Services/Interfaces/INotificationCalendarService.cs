using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.FunidngClaims.Signing.Services.Interfaces
{
    public interface INotificationCalendarService
    {
        Task<bool> IsSigningNotificationPollRequiredAsync(CancellationToken cancellationToken);
    }
}
