namespace Beis.HelpToGrow.Web.Services.Interface
{
    using System.Threading.Tasks;

    using Beis.HelpToGrow.Web.Models;

    public interface IVendorCompanyService
    {
        Task<VendorCompanyViewModel> GetVendorCompany(long id);
    }
}