using Beis.Htg.VendorSme.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Beis.VendorManagement.Repositories.Interface
{
    public interface IManageUsersRepository
    {
        Task<IEnumerable<vendor_company_user>> GetAllUsers(string adb2CId);

        Task AddUser(vendor_company_user user);
        
        Task UpdateUser(long userId, string fullName);

        Task UpdateAdb2CForUser(long userId, string adb2CId);

        Task<vendor_company_user> GetUserSingle(long userId);

        Task<vendor_company_user> GetUserSingle(string adb2CId);

        Task<vendor_company_user> GetUserByEmailAndCompanyIdSingle(string email, long companyId);

        Task<vendor_company_user> GetUserByEmail(string email);

        Task DeleteUser(long userId);
        
        Task UpdatePrimaryContact(long id, long companyId);

        Task<vendor_company_user> GetPrimaryUserAsync(long companyId);
    }
}