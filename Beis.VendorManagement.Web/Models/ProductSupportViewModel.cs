namespace Beis.VendorManagement.Web.Models
{
    public class ProductSupportViewModel : BaseViewModel
    {
        public ProductSupportViewModel()
        {
            SettingsProductFiltersCategories = new List<SettingsProductFiltersCategory>();
        }

        public long ProductId { get; set; }

        public string Adb2CId { get; set; }

        public string ProductName { get; set; }

        public ProductStatus ProductStatus { get; set; }

        public IList<SettingsProductFiltersCategory> SettingsProductFiltersCategories { get; set; }
    }

    public class SettingsProductFiltersCategory
    {
        public string ItemName { get; set; }

        public IList<SelectListItem> SettingsProductFilters { get; set; }
    }
}