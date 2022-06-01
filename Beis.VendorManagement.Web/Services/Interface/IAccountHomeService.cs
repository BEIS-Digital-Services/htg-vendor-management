namespace Beis.VendorManagement.Web.Services.Interface
{
    public interface IAccountHomeService
    {
        Task<bool> UpdateCompanyIpAddress(string abd2CId, string ipRange);
        
        Task<VendorCompanyViewModel> GetCompanyByUserIdAsync(string abd2CId);
    }
}