using Beis.VendorManagement.Web.Models;
using System.Threading.Tasks;

namespace Beis.VendorManagement.Web.Services.Interface
{
    public interface IVendorCompanyService
    {
        Task<VendorCompanyViewModel> GetVendorCompany(long id);
    }
}