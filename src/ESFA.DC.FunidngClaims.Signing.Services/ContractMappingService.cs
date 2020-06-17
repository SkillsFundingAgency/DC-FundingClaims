using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ReferenceData.FCS.Model;
using ESFA.DC.ReferenceData.FCS.Model.FCS;
using ESFA.DC.ReferenceData.FCS.Service.Interface;

namespace ESFA.DC.ReferenceData.FCS.Service
{
    public class ContractMappingService : IContractMappingService
    {
        public Contractor Map(Guid syndicationItemId, contract contract)
        {
            if (contract.contractor == null)
            {
                throw new ArgumentNullException("Contractor Missing for Contract");
            }

            var contractor = MapContractor(contract.contractor);

            contractor.SyndicationItemId = syndicationItemId;

            contractor.Contracts = FlattenContracts(contract).Where(c => c.hierarchyType == hierarchyType.CONTRACT).Select(MapContract).ToList();

            return contractor;
        }

        public Contractor MapContractor(contractor contractor)
        {
            return new Contractor()
            {
                OrganisationIdentifier = contractor.organisationIdentifier,
                Ukprn = contractor.ukprn,
                LegalName = contractor.legalName,
            };
        }

        public Contract MapContract(contractType contract)
        {
            IEnumerable<ContractAllocation> contractAllocations = new List<ContractAllocation>();

            if (contract.contractAllocations != null)
            {
                contractAllocations = contract.contractAllocations.SelectMany(FlattenContractAllocations).Select(ca => MapContractAllocation(ca, contract.contractor));
            }

            return new Contract()
            {
                ContractNumber = contract.contractNumber,
                ContractVersionNumber = contract.contractVersionNumber,
                StartDate = contract.startDateSpecified ? contract.startDate : null,
                EndDate = contract.endDateSpecified ? contract.endDate : null,
                ContractAllocations = contractAllocations.ToList()
            };
        }

        public ContractAllocation MapContractAllocation(contractAllocationsContractAllocation contractAllocation, contractor contractor)
        {
            IEnumerable<ContractDeliverable> contractDeliverables = new List<ContractDeliverable>();

            if (contractAllocation.contractDeliverables != null)
            {
                contractDeliverables = contractAllocation.contractDeliverables.SelectMany(FlattenContractDeliverables).Select(MapContractDeliverable);
            }

            return new ContractAllocation()
            {
                ContractAllocationNumber = contractAllocation.contractAllocationNumber,
                FundingStreamCode = contractAllocation.fundingStream?.fundingStreamCode,
                FundingStreamPeriodCode = contractAllocation.fundingStreamPeriodCode,
                Period = contractAllocation.period?.period1,
                PeriodTypeCode = contractAllocation.period?.periodType?.periodTypeCode.ToString(),
                LearningRatePremiumFactor = contractAllocation.ProcurementAttrs?.LearningRatePremium,
                UoPcode = contractAllocation.uopCode,
                StartDate = contractAllocation.startDateSpecified ? contractAllocation.startDate : default(DateTime?),
                EndDate = contractAllocation.endDateSpecified ? contractAllocation.endDate : null,
                TenderSpecReference = contractAllocation.ProcurementAttrs?.TenderSpecReference,
                LotReference = contractAllocation.ProcurementAttrs?.LotReference,
                DeliveryOrganisation = contractAllocation.allocationOrganisationRelationships?.Organisation?.organisationIdentifier ?? contractor.organisationIdentifier,
                DeliveryUkprn = contractAllocation.allocationOrganisationRelationships?.Organisation?.ukprn ?? contractor.ukprn,
                TerminationDate = contractAllocation.allocationTerminationAttrs?.terminationDate,
                StopNewStartsFromDate = contractAllocation.allocationTerminationAttrs?.stopNewStartsFromDate,
                MaxContractValue = contractAllocation.maximumContractValueSpecified ? contractAllocation.maximumContractValue : (decimal?)null,
                ContractDeliverables = contractDeliverables.ToList()
            };
        }

        public ContractDeliverable MapContractDeliverable(contractDeliverablesTypeContractDeliverable contractDeliverable)
        {
            return new ContractDeliverable()
            {
                DeliverableCode = contractDeliverable.deliverable?.deliverableCode,
                Description = contractDeliverable.deliverableDescription ?? contractDeliverable.deliverable?.deliverableName,
                PlannedValue = contractDeliverable.plannedValueSpecified ? contractDeliverable.plannedValue : default(decimal?),
                PlannedVolume = contractDeliverable.plannedVolumeSpecified ? contractDeliverable.plannedVolume : default(int?),
                UnitCost = contractDeliverable.unitCostSpecified ? contractDeliverable.unitCost : default(decimal?)
            };
        }

        public IEnumerable<contractType> FlattenContracts(contractType contract)
        {
            return contract.SelectRecursive(c => c.contracts);
        }

        public IEnumerable<contractAllocationsContractAllocation> FlattenContractAllocations(contractAllocationsContractAllocation contractAllocation)
        {
            return contractAllocation.SelectRecursive(ca => ca.contractAllocations);
        }

        public IEnumerable<contractDeliverablesTypeContractDeliverable> FlattenContractDeliverables(contractDeliverablesTypeContractDeliverable contractDeliverable)
        {
            return contractDeliverable.SelectRecursive(d => d.contractDeliverables);
        }
    }
}
