namespace Beis.VendorManagement.Repositories.Interface
{
    public interface ISettingsProductCapabilitiesRepository
    {
        Task<IList<settings_product_capability>> GetSettingsProductCapabilities();
    }
}