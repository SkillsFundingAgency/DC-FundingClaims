using System.Collections.Generic;

namespace ESFA.DC.FundingClaims.Data.Entities
{
    public partial class SubmissionTypes
    {
        public SubmissionTypes()
        {
            FundingClaimsSubmissionFile = new HashSet<FundingClaimsSubmissionFile>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<FundingClaimsSubmissionFile> FundingClaimsSubmissionFile { get; set; }
    }
}
