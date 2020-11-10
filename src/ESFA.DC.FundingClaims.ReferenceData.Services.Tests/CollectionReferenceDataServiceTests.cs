using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.FundingClaims.Data;
using ESFA.DC.FundingClaims.Data.Entities;
using ESFA.DC.Logging.Interfaces;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ESFA.DC.FundingClaims.ReferenceData.Services.Tests
{
    public class CollectionReferenceDataServiceTest
    {
        [Fact]
        public async Task GetCollectionsOpenByDateRangeAsync_OnlyReturnsCollectionsStillOpenWithinDateRange()
        {
            // Arrange
            var collections = new List<CollectionDetail>
            {
                new CollectionDetail
                {
                    SubmissionOpenDateUtc = new DateTime(2019,1,1),
                    SubmissionCloseDateUtc = new DateTime(2019,1,10),
                    CollectionYear = 1920
                },
                new CollectionDetail
                {
                    SubmissionOpenDateUtc = new DateTime(2020,1,1),
                    SubmissionCloseDateUtc = new DateTime(2020,1,10),
                    CollectionYear = 2021
                }
            };

            var contextMock = BuildContextMock(collections);
            var sut = new CollectionReferenceDataService(contextMock, new Mock<IDateTimeProvider>().Object, new Mock<ILogger>().Object);

            // Act
            var result = await sut.GetCollectionsOpenByDateRangeAsync(CancellationToken.None, new DateTime(2019, 1, 1), new DateTime(2019, 1, 10));

            // Assert
            result.Count().Should().Be(1);
            result.First().CollectionYear.Should().Be(collections.First().CollectionYear);
        }

        private Func<IFundingClaimsDataContext> BuildContextMock(List<CollectionDetail> collections) 
        {
            var contextMock = new Mock<IFundingClaimsDataContext>();

            contextMock.Setup(m => m.CollectionDetail).Returns(collections.AsQueryable().BuildMockDbSet().Object);

            return () => contextMock.Object;
        }
    }
}
