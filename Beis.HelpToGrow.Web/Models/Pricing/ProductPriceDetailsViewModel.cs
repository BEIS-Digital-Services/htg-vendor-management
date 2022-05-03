using System.Collections.Generic;

namespace Beis.VendorManagement.Web.Models.Pricing
{
    public class ProductPriceDetailsViewModel
    {
        public long ProductId { get; set; }
        
        public string ProductName { get; set; }
        
        public string Adb2CId { get; set; }
        
        public IEnumerable<ProductPriceViewModel> ProductPrices { get; set; }
    }
}