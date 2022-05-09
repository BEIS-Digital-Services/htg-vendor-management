using Beis.Htg.VendorSme.Database;
using Beis.Htg.VendorSme.Database.Models;
using Beis.VendorManagement.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Beis.VendorManagement.Repositories
{
    public class ProductFiltersRepository : IProductFiltersRepository
    {
        private readonly HtgVendorSmeDbContext _context;

        public ProductFiltersRepository(HtgVendorSmeDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<product_filter>> GetProductFilters(long productId)
        {
            return await _context.product_filters.Where(pf => pf.product_id == productId).ToListAsync();
        }

        public async Task DeleteAllProductFilters(long productId, IEnumerable<long> filterTypes)
        {
            var filterIds = _context.settings_product_filters.Where(x => filterTypes.Contains(x.filter_type)).Select(y => y.filter_id);

            _context.product_filters.RemoveRange(_context.product_filters.Where(x => x.product_id == productId && filterIds.Contains(x.filter_id)));
            await _context.SaveChangesAsync();
        }

        public async Task AddProductFilters(IEnumerable<product_filter> productFilters)
        {
            foreach (var filter in productFilters)
            {
                await _context.product_filters.AddAsync(filter);
                await _context.SaveChangesAsync();
            }
        }
    }
}