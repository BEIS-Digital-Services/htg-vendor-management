namespace Beis.VendorManagement.Repositories.Interface
{
    public interface ISettingsProductFiltersRepository
    {
        Task<IList<settings_product_filter>> GetSettingsProductFilters(long filterType);
    }
}