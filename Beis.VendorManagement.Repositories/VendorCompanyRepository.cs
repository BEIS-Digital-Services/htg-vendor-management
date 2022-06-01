namespace Beis.VendorManagement.Repositories
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