namespace Beis.HelpToGrow.Web.Models
{
    using System.Collections.Generic;

    using Beis.HelpToGrow.Web.Models.Pricing;

    public class ProductPriceDetailsViewModel
    {
        public long ProductId { get; set; }
        
        public string ProductName { get; set; }
        
        public string Adb2CId { get; set; }
        
        public IEnumerable<ProductPriceViewModel> ProductPrices { get; set; }
    }
}