using Beis.HelpToGrow.Core.Repositories.Interface;
using Beis.Htg.VendorSme.Database;
using Beis.Htg.VendorSme.Database.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Beis.HelpToGrow.Core.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly HtgVendorSmeDbContext _context;

        public ProductRepository(HtgVendorSmeDbContext context)
        {
            _context = context;
        }

        public async Task UpdateProduct(product product)
        {
            _context.products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task<product> GetProductSingle(long id, string adb2CId)
        {
            var vendorId = (await _context.vendor_company_users.FirstOrDefaultAsync(x => x.adb2c == adb2CId))?.companyid;
            return await _context.products.FirstOrDefaultAsync(t => t.product_id == id && t.vendor_id == vendorId);
        }
    }
}