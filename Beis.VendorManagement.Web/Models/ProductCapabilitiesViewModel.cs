namespace Beis.VendorManagement.Web.Models
{
    public class ProductCapabilitiesViewModel : BaseViewModel
    {
        public long ProductId { get; set; }
        
        public string ProductName { get; set; }
        
        public string DraftAdditionalCapabilities { get; set; }
        
        public IList<SelectListItem> SettingsProductCapabilitiesList { get; set; }
        
        public ProductStatus Status { get; set; }

        public string Adb2CId { get; set; }

        public long ProductTypeId { get; set; }
    }
}