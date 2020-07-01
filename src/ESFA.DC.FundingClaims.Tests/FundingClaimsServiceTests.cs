using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.FundingClaims.Services;
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
        [Fact]
        public void Test()
        {
            Assert.True(true);
        }
       
    }
}
