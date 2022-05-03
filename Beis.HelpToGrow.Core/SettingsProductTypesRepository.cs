using Beis.Htg.VendorSme.Database;
using Beis.Htg.VendorSme.Database.Models;
using Beis.VendorManagement.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Beis.VendorManagement.Repositories
{
    public class SettingsProductTypesRepository : ISettingsProductTypesRepository
    {
        private readonly HtgVendorSmeDbContext _context;

        public SettingsProductTypesRepository(HtgVendorSmeDbContext context)
        {
            _context = context;
        }

        public async Task<IList<settings_product_type>> GetSettingsProductTypes()
        {
            return await _context.settings_product_types.ToListAsync();
        }

        public async Task<string> GetSettingsProductType(long productId)
        {
            var type = await _context.settings_product_types.FirstOrDefaultAsync(c => c.id == productId);
            return type == null ? string.Empty : type.item_name;
        }
    }
}