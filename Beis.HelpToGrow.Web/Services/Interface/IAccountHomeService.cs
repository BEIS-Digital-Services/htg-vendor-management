namespace Beis.HelpToGrow.Web.Services.Interface
{
    using System.Threading.Tasks;

    using Beis.HelpToGrow.Web.Models;

    public interface IAccountHomeService
    {
        Task<bool> UpdateCompanyIpAddress(string abd2CId, string ipRange);
        
        Task<VendorCompanyViewModel> GetCompanyByUserIdAsync(string abd2CId);
    }
}