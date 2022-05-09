using System.Collections.Generic;

namespace Beis.VendorManagement.Web.Models.Pricing
{
    public class AdditionalCostsViewModel
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
    }

    public class AdditionalCostDetail
    {
        public string Type { get; set; }
        
        public string CostAndFrequency { get; set; }
        
        public bool IsMandatory { get; set; }
    }
}