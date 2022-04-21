namespace Beis.HelpToGrow.Web.Models.Pricing
{
    public class PricingHomeViewModel
    {
        public long ProductId { get; set; }
     
        public long ProductPriceId { get; set; }
        
        public string ProductName { get; set; }
        
        public string Adb2CId { get; internal set; }
    }
}