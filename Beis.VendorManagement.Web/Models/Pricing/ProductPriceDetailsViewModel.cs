namespace Beis.VendorManagement.Web.Models.Pricing
{
    public class ProductPriceDetailsViewModel : BaseViewModel
    {
        public long ProductId { get; set; }

        public string ProductName { get; set; }

        public string Adb2CId { get; set; }

        public IEnumerable<ProductPriceViewModel> ProductPrices { get; set; }
    }
}