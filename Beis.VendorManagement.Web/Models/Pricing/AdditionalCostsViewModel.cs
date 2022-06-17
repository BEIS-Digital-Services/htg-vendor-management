using Beis.VendorManagement.Web.Extensions;

namespace Beis.VendorManagement.Web.Models.Pricing
{
    public class AdditionalCostsViewModel : BaseViewModel
    {
        public AdditionalCostsViewModel()
        {
            AdditionalCosts = new List<AdditionalCostDetail>();
        }

        public long ProductPriceId { get; set; }
        
        public long ProductId { get; set; }

        public string Adb2CId { get; set; }

        public string ProductName { get; set; }
        
        public IList<AdditionalCostDetail> AdditionalCosts { get; set; }

        public IEnumerable<AdditionalCostDetail> GetAdditionalCostsByType(EnumAdditionalCostType additionalCostType)
        {
            return this.AdditionalCosts
                .Where(x => x.AdditionalCostType == additionalCostType)
                .ToList(); // View enumerates this several times
        }
    }

    public class AdditionalCostDetail
    {
        public string Type { get; set; }
        
        public string CostAndFrequency { get; set; }
        
        public bool IsMandatory { get; set; }
		
        public EnumAdditionalCostType? AdditionalCostType { get; set; }
	}
}