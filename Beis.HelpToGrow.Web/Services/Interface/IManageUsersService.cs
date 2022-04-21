using Beis.HelpToGrow.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Beis.HelpToGrow.Web.Services.Interface
{
    public interface IManageUsersService
    {
        Task<IList<VendorCompanyUserViewModel>> GetAllUsers(string adb2CId);
     
        Task<VendorCompanyUserViewModel> GetUser(string adb2CId);

        Task<VendorCompanyUserViewModel> GetUser(long userId);
    }
}