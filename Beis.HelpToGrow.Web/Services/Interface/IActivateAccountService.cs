using Beis.HelpToGrow.Web.Models;
using System.Threading.Tasks;

namespace Beis.HelpToGrow.Web.Services.Interface
{
    public interface IActivateAccountService
    {
        Task<VendorCompanyViewModel> GetCompanyByUserIdAsync(long userId);

        Task<VendorCompanyUserViewModel> GetUserById(long userId);
        
        Task<VendorCompanyUserViewModel> GetUserByAccessLink(string accessLink);
    }
}