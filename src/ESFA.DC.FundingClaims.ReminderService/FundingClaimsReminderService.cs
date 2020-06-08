using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.FundingClaims.Interfaces;
using ESFA.DC.FundingClaims.ReminderService.Interfaces;
using ESFA.DC.JobNotifications.Interfaces;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.FundingClaims.ReminderService
{
    public class FundingClaimsReminderService : IFundingClaimsReminderService
    {
        private readonly ILogger _logger;
        private readonly IEmailNotifier _emailNotifier;
        private readonly ICollectionService _collectionService;
        private readonly IFundingClaimsEmailService _fundingClaimsEmailService;
        private readonly ICollectionEmailTemplateManager _collectionEmailTemplateManager;
        
        public FundingClaimsReminderService(
            IEmailNotifier emailNotifier,
            ICollectionService collectionService,
            IFundingClaimsEmailService fundingClaimsEmailService,
            ICollectionEmailTemplateManager collectionEmailTemplateManager,
            ILogger logger)
        {
            _emailNotifier = emailNotifier;
            _collectionService = collectionService;
            _fundingClaimsEmailService = fundingClaimsEmailService;
            _collectionEmailTemplateManager = collectionEmailTemplateManager;
            _logger = logger;
        }

        public async Task Execute()
        {
            _logger.LogInfo("Funding Claims Email Reminder Service Started.");

            var numberOfEmailsSent = await SendEmails();

            _logger.LogInfo($"Funding Claims Email Reminder Service Stopped.{Environment.NewLine}Number of email reminders sent: {numberOfEmailsSent}");
        }

        private async Task<int> SendEmails()
        {
            const string dateFormatString = "h tt d MMMM";
            int emailsSentCount = 0;

            var collection = await _collectionService.GetFundingClaimsCollection();
            if (collection == null)
            {
                _logger.LogDebug("Collection closed, no emails to send");
                return 0;
            }


            var emailTemplate = await _collectionEmailTemplateManager.GetTemplate(collection.CollectionId); 

            if (emailTemplate == null)
            {
                _logger.LogError("No valid Active Email Template for Collection");
                return 0;
            }

            var emails = (await _fundingClaimsEmailService.GetUnsubmittedClaimEmailAddressses(collection.CollectionCode,collection.CollectionYear.ToString(), collection.SubmissionOpenDateUtc)).ToList();

            if (!emails.Any())
            {
                _logger.LogDebug("No emails to send, as there are no providers in draft state who have not submitted");
                return 0;
            }

            foreach (var email in emails)
            {
                var parameters = new Dictionary<string, dynamic>
                {
                    {"SubmissionCloseDate", collection.SubmissionCloseDateUtc.ToString(dateFormatString)},
                    {"SignatureCloseDate", collection.SignatureCloseDateUtc.GetValueOrDefault().ToString(dateFormatString)},
                };

                try
                {
                    await _emailNotifier.SendEmail(email, emailTemplate, parameters);
                    emailsSentCount++;
                }
                catch (Notify.Exceptions.NotifyClientException e)
                {
                    _logger.LogError($"Error sending email to email address: {email}", e);
                }
            }

            return emailsSentCount;
        }
    }
}
