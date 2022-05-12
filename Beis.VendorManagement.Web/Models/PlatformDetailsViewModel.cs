using Beis.VendorManagement.Web.Models.Enums;

namespace Beis.VendorManagement.Web.Models
{
    public class PlatformDetailsViewModel : BaseViewModel
    {
        public long ProductId { get; set; }
     
        public string Adb2CId { get; set; }
        
        public string ProductName { get; set; }
        
        public string ReviewUrl { get; set; }
        
        public string OtherCompatibility { get; set; }
 
        public ProductStatus ProductStatus { get; set; }

        public SettingsProductFiltersCategory SettingsProductFiltersCategory { get; set; }
    }
}