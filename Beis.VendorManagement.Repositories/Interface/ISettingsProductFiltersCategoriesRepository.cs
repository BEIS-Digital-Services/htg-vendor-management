namespace Beis.VendorManagement.Repositories.Interface
{
    public interface ISettingsProductFiltersCategoriesRepository
    {
        Task<IList<settings_product_filters_category>> GetSettingsProductFiltersCategories();
    }
}