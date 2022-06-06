namespace Beis.VendorManagement.Web.Models
{
    public class ProductSubmitReviewDetailsViewModel : BaseViewModel
    {
        public long ProductId { get; set; }

        public string Adb2CId { get; set; }

        public string DraftProductDescription { get; set; }
        
        public string RedemptionUrl { get; set; }
        
        public IList<string> ProductCapabilities { get; set; }
        
        public string ProductLogo { get; set; }
        
        public string ProductName { get; set; }
        
        public IList<string> SupportItems { get; set; }
        
        public IList<string> TrainingItems { get; set; }
        
        public IList<string> PlatformItems { get; set; }
        
        public string DraftOtherCompatibility { get; set; }
        
        public string DraftReviewUrl { get; set; }
    }
}