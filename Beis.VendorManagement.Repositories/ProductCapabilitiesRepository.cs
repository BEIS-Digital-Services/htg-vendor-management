namespace Beis.VendorManagement.Repositories
{
    public class ProductCapabilitiesRepository : IProductCapabilitiesRepository

    {
        private readonly HtgVendorSmeDbContext _context;

        public ProductCapabilitiesRepository(HtgVendorSmeDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<product_capability>> GetProductCapabilitiesFilters(long productId = 0)
        {
            if (productId > 0)
            {
                return await _context.product_capabilities.Where(x => x.product_id == productId).ToListAsync();
            }

            return await _context.product_capabilities.ToListAsync();
        }

        public async Task DeleteAllProductCapabilitiesFilters(long productId, long productType)
        {
            var filterCapabilitiesIds = _context.settings_product_capabilities.Where(x => x.product_type == productType).Select(y => y.capability_id);

            _context.product_capabilities.RemoveRange(_context.product_capabilities.Where(x => x.product_id == productId && filterCapabilitiesIds.Contains(x.capability_id)));
            await _context.SaveChangesAsync();
        }

        public async Task AddProductCapabilitiesFilters(IEnumerable<product_capability> product_capabilities)
        {
            foreach (var filter in product_capabilities)
            {
                await _context.product_capabilities.AddAsync(filter);
            }
            await _context.SaveChangesAsync();
        }
    }
}