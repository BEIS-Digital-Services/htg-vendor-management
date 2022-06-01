namespace Beis.VendorManagement.Repositories
{
    public class CompanyUserRepository : ICompanyUserRepository
    {
        private readonly HtgVendorSmeDbContext _context;

        public CompanyUserRepository(HtgVendorSmeDbContext context)
        {
            _context = context;
        }
       
        public async Task<vendor_company> GetCompanyByUserSingle(string adb2CId)
        {
            var user = await _context.vendor_company_users.FirstOrDefaultAsync(u => u.adb2c == adb2CId);
            return user != null ? await _context.vendor_companies.FirstOrDefaultAsync(t => t.vendorid == user.companyid) : null;
        }

        public async Task<vendor_company> GetCompanyByUserSingle(long userId)
        {
            var user = await _context.vendor_company_users.FirstOrDefaultAsync(u => u.userid == userId);
            return user != null ? await _context.vendor_companies.FirstOrDefaultAsync(t => t.vendorid == user.companyid) : null;
        }

        public async Task<IList<product>> GetProductsByUserIdSingle(long id)
        {
            var products = new List<product>();

            var user = await _context.vendor_company_users.FirstOrDefaultAsync(u => u.userid == id);
            if (user == null) return products;

            var company = await _context.vendor_companies.FirstOrDefaultAsync(c => c.vendorid == user.companyid);
            if (company == null) return products;

            products = await _context.products.Where(p => p.vendor_id == company.vendorid).ToListAsync<product>();
            return products;
        }

        public async Task<vendor_company_user> GetUserByIdSingle(long id)
        {
            return await _context.vendor_company_users.Where(u => u.userid == id).FirstOrDefaultAsync();
        }

        public async Task<vendor_company_user> GetUserByAccessLink(string accessLink)
        {
            return await _context.vendor_company_users.Where(u => u.access_link.Trim().ToLower() == accessLink.Trim().ToLower()).FirstOrDefaultAsync();
        }

        public async Task<vendor_company_user> GetUserIdByAdb2CUserId(string id)
        {
            return await _context.vendor_company_users.Where(u => u.adb2c == id).FirstOrDefaultAsync();
        }

        public async Task UpdateCompany(vendor_company company)
        {
            _context.vendor_companies.Update(company);
            await _context.SaveChangesAsync();
        }
    }
}