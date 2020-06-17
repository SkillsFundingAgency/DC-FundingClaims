using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.FuncingClaims.Services.Interfaces;
using ESFA.DC.FundingClaims.Data;
using ESFA.DC.FundingClaims.Data.Entities;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.ReferenceData.Organisations.Model.Interface;
using FluentAssertions;
using Moq;
using Xunit;
using MockQueryable.Moq;
using FundingClaimsSupportingData = ESFA.DC.FundingClaims.Data.Entities.FundingClaimsSupportingData;

namespace ESFA.DC.FundingClaims.Tests
{
    public class FundingClaimsServiceTests
    {
        //[Fact]
        //public async Task GetUnsubmittedClaims_Returns_SuccessfulMessages()
        //{
        //    var fundingClaimsSupportingDataList = new List<FundingClaimsSupportingData>()
        //    {
        //        new FundingClaimsSupportingData()
        //        {
        //            UserEmailAddress = "test.com",
        //            CollectionCode = "FC03",
        //            LastUpdatedDateTimeUtc = DateTime.Now.AddDays(-1),
        //            Ukprn = 123,
        //        }
        //    };

        //    var fundingClaimsSupportingDataDb = fundingClaimsSupportingDataList.AsQueryable().BuildMockDbSet();

        //    var fundingClaimsSubmissionFileList = new List<FundingClaimsSubmissionFile>()
        //    {
        //        new FundingClaimsSubmissionFile()
        //    };

        //    var fundingClaimsSubmissionFileDb = fundingClaimsSubmissionFileList.AsQueryable().BuildMockDbSet();

        //    var fundingClaimsDataContextMock = new Mock<IFundingClaimsDataContext>();
            
        //    fundingClaimsDataContextMock.Setup(s => s.FundingClaimsSubmissionFile)
        //        .Returns(fundingClaimsSubmissionFileDb.Object);

        //    fundingClaimsDataContextMock.Setup(s => s.FundingClaimsSupportingData)
        //        .Returns(fundingClaimsSupportingDataDb.Object);
            
        //    Func<IFundingClaimsDataContext> fundingClaimsDataContextFactory = () => fundingClaimsDataContextMock.Object;

        //    var fundingClaimsCollectionMetaDataList = new List<FundingClaimsCollectionMetaData>()
        //    {
        //        new FundingClaimsCollectionMetaData()
        //        {
        //            CollectionCode = "FC03",
        //            SubmissionCloseDateUtc = DateTime.Now,
        //            SignatureCloseDateUtc = DateTime.Now,
        //            CollectionId = 1
        //        }
        //    };

        //    var fundingClaimsCollectionMetaDataDb = fundingClaimsCollectionMetaDataList.AsQueryable().BuildMockDbSet();

        //    var emailTemplateLIst = new List<JobEmailTemplate>()
        //    {
        //        new JobEmailTemplate()
        //        {
        //            CollectionId = 1,
        //            Active = true,
        //            TemplateOpenPeriod = "123"
        //        }
        //    };

        //    var emailTemplateDb = emailTemplateLIst.AsQueryable().BuildMockDbSet();

        //    var jobQueueDataContextMock = new Mock<IJobQueueDataContext>();
        //    jobQueueDataContextMock.Setup(s => s.FundingClaimsCollectionMetaData)
        //        .Returns(fundingClaimsCollectionMetaDataDb.Object);
        //    jobQueueDataContextMock.Setup(s => s.JobEmailTemplate).Returns(emailTemplateDb.Object);

        //    Func<IJobQueueDataContext> jobQueueDataContextFactory = () => jobQueueDataContextMock.Object;

        //    var ilr1819RulebaseContextMock = new Mock<IIlr1819RulebaseContext>();
        //    Func<IIlr1819RulebaseContext> ilr1819RulebaseContextFactory = () => ilr1819RulebaseContextMock.Object;

        //    var fcsContextMock = new Mock<IFcsContext>();
        //    Func<IFcsContext> fcsContextFactory = () => fcsContextMock.Object;

        //    var organisationContextMock = new Mock<IOrganisationsContext>();
        //    Func<IOrganisationsContext> organisationContextFactory = () => organisationContextMock.Object; 
            
        //    var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        //    var mockFundingClaimsMessagingService = new Mock<IFundingClaimsMessagingService>();
        //    var mockLogger = new Mock<ILogger>();

        //    mockDateTimeProvider.Setup(s => s.GetNowUtc()).Returns(DateTime.Now);
        //}
    }
}
