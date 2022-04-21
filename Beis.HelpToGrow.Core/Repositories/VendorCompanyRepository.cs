using Beis.HelpToGrow.Core.Repositories.Interface;
using Beis.Htg.VendorSme.Database;
using Beis.Htg.VendorSme.Database.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Beis.HelpToGrow.Core.Repositories
{
    public class VendorCompanyRepository : IVendorCompanyRepository
    {
        private readonly HtgVendorSmeDbContext _context;

        public VendorCompanyRepository(HtgVendorSmeDbContext context)
        {
            _context = context;
        }

        public async Task<vendor_company> GetVendorCompanySingle(long id)
        {
            return await _context.vendor_companies.FirstOrDefaultAsync(t => t.vendorid == id);
        }
    }
}