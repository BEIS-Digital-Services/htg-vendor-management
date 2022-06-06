namespace Beis.VendorManagement.Web.Extensions
{
    internal static class SettingsProductFilterCategoryExtensions
    {
        internal static IList<SettingsProductFiltersCategory> GetSettingsProductFilterCategory(
            this IList<settings_product_filters_category> settingsProductFiltersCategories,
            IList<settings_product_filter> settingsProductFilters, IEnumerable<product_filter> productFilters)
        {
            var newSettingsProductFilterCategories = new List<SettingsProductFiltersCategory>();
            foreach (var settingsProductFiltersCategory in settingsProductFiltersCategories)
            {
                var items = settingsProductFilters
                    .Where(x => x.filter_type == settingsProductFiltersCategory.id).Select(x => new SelectListItem
                    {
                        Text = x.filter_name,
                        Value = x.filter_id.ToString()
                    });

                //Assign existing filters
                var lstItems = items.ToList();
                foreach (var productFilter in productFilters)
                {
                    foreach (var lstItem in lstItems.Where(lstItem => lstItem.Value == productFilter.filter_id.ToString()))
                    {
                        lstItem.Selected = true;
                    }
                }

                newSettingsProductFilterCategories.Add(new SettingsProductFiltersCategory
                {
                    ItemName = settingsProductFiltersCategory.item_name,
                    SettingsProductFilters = lstItems
                });
            }

            return newSettingsProductFilterCategories;
        }
    }
}