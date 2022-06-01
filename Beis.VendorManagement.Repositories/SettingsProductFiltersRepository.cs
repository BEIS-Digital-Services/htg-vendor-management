namespace Beis.VendorManagement.Repositories
{
    public class SettingsProductFiltersRepository : ISettingsProductFiltersRepository

    {
        private readonly HtgVendorSmeDbContext _context;

        public SettingsProductFiltersRepository(HtgVendorSmeDbContext context)
        {
            _context = context;
        }

        public async Task<IList<settings_product_filter>> GetSettingsProductFilters(long filterType)
        {
            var result = filterType > 0
                ? await _context.settings_product_filters.Where(x => x.filter_type == filterType).OrderBy(x=> x.sort_order).ToListAsync()
                : await _context.settings_product_filters.OrderBy(x => x.sort_order).ToListAsync();

            return result;
        }
    }
}