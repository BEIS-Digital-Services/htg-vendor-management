namespace Beis.HelpToGrow.Web.Models
{
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Mvc.Rendering;

    public class ProductSupportViewModel
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