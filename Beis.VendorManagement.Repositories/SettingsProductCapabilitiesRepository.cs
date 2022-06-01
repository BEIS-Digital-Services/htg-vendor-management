namespace Beis.VendorManagement.Repositories
{
    public class SettingsProductCapabilitiesRepository : ISettingsProductCapabilitiesRepository

    {
        private readonly HtgVendorSmeDbContext _context;

        public SettingsProductCapabilitiesRepository(HtgVendorSmeDbContext context)
        {
            _context = context;
        }

        public async Task<IList<settings_product_capability>> GetSettingsProductCapabilities()
        {
            return await _context.settings_product_capabilities.ToListAsync();
        }
    }
}