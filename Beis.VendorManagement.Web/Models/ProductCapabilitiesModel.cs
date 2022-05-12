namespace Beis.VendorManagement.Web.Models
{
    public class ProductCapabilitiesModel : BaseViewModel
    {
        public long ProductId { get; set; }

        public long CapabilityId{ get; set; }

        public string DraftCapability { get; set; }
    }
}