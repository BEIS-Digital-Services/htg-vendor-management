namespace Beis.VendorManagement.Web.Models.Pricing
{
    public class PricingHomeViewModel: BaseViewModel
    {
        public long ProductId { get; set; }
     
        public long ProductPriceId { get; set; }
        
        public string ProductName { get; set; }
        
        public string Adb2CId { get; internal set; }
    }
}