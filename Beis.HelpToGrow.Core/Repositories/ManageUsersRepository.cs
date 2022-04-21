using Beis.HelpToGrow.Core.Repositories.Interface;
using Beis.Htg.VendorSme.Database;
using Beis.Htg.VendorSme.Database.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Beis.HelpToGrow.Core.Repositories
{
    public class ManageUsersRepository : IManageUsersRepository
    {
        private readonly HtgVendorSmeDbContext _context;

        public ManageUsersRepository(HtgVendorSmeDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<vendor_company_user>> GetAllUsers(string adb2CId)
        {
            var companyId = (await _context.vendor_company_users.SingleOrDefaultAsync(c => c.adb2c == adb2CId))?.companyid;
            return companyId == null
                ? Enumerable.Empty<vendor_company_user>()
                : await _context.vendor_company_users.Where(c => c.companyid == companyId).ToListAsync();
        }

        public async Task<vendor_company_user> GetUserSingle(long userId)
        {
            return await _context.vendor_company_users.SingleOrDefaultAsync(u => u.userid == userId);
        }

        public async Task<vendor_company_user> GetUserSingle(string adb2CId)
        {
            return await _context.vendor_company_users.SingleOrDefaultAsync(u => u.adb2c == adb2CId);
        }

        public async Task<vendor_company_user> GetUserByEmailAndCompanyIdSingle(string email, long companyId)
        {
            return await _context.vendor_company_users.FirstOrDefaultAsync(u => string.Compare(u.email.ToLower(), email.ToLower()) == 0 && u.companyid == companyId);
        }

        public async Task<vendor_company_user> GetUserByEmail(string email)
        {
            return await _context.vendor_company_users.SingleOrDefaultAsync(u => u.email == email);
        }

        public async Task AddUser(vendor_company_user user)
        {
            user.contact = user.position = string.Empty;
            await _context.vendor_company_users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUser(long userId, string fullName)
        {
            _context.vendor_company_users.Single(u => u.userid == userId).full_name = fullName;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAdb2CForUser(long userId, string adb2CId)
        {
            var currentUser = _context.vendor_company_users.Single(u => u.userid == userId);
            currentUser.access_link = string.Empty;
            currentUser.adb2c = adb2CId;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUser(long userId)
        {
            var entity = await _context.vendor_company_users.FirstOrDefaultAsync(u => u.userid == userId);
            _context.vendor_company_users.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePrimaryContact(long id, long companyId)
        {
            var users = await _context.vendor_company_users.Where(u => u.companyid == companyId).ToListAsync<vendor_company_user>();
            users.Single(u => u.primary_contact).primary_contact = false;
            users.Single(u => u.userid == id).primary_contact = true;
            await _context.SaveChangesAsync();
        }

        public async Task<vendor_company_user> GetPrimaryUserAsync(long companyId)
        {
            return await _context.vendor_company_users.Where(c => c.companyid == companyId && c.primary_contact).FirstOrDefaultAsync();
        }
    }
}