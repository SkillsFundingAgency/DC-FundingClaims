using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Data;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.FundingClaims.ReminderService.Services
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly Func<IJobQueueDataContext> _jobQueueDataContextFactory;

        public EmailTemplateService(Func<IJobQueueDataContext> jobQueueDataContextFactory)

        {
            _jobQueueDataContextFactory = jobQueueDataContextFactory;
        }

        public async Task<string> GetEmailTemplateAsync(CancellationToken cancellationToken, int collectionId)
        {
            using (IJobQueueDataContext context = _jobQueueDataContextFactory())
            {
                var emailTemplate = await
                    context.JobEmailTemplate.SingleOrDefaultAsync(x => x.CollectionId == collectionId
                                                                       && x.Active.Value, cancellationToken);

                return emailTemplate?.TemplateOpenPeriod ?? string.Empty;
            }
        }
    }
}
