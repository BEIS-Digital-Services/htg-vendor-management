namespace Beis.VendorManagement.Web.Models.Pricing
{
    public class UserDiscountViewModel
    {
        public long? ProductPriceId { get; set; }

        public int MaxLicenses { get; set; }

        public int MinLicenses { get; set; }

        public decimal DiscountPrice { get; set; }
        
        public decimal DiscountPercentage { get; set; }
        
        public string DiscountSku { get; set; }
    }
}