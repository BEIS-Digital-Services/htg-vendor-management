using Beis.VendorManagement.Web.Models.Enums;

namespace Beis.VendorManagement.Web.Models
{
    public class SummaryViewModel
    {
        public long ProductId { get; set; }

        public string ProductName { get; set; }

        public string DraftProductDescription { get; set; }

        public string Adb2CId { get; set; }

        public ProductStatus ProductStatus { get; set; }
    }
}