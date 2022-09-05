using Beis.VendorManagement.Web.Mappers;

namespace Beis.VendorManagement.Web.Services
{
    public class AccountHomeService : IAccountHomeService
    {
        private readonly ICompanyUserRepository _companyUserRepository;

        public AccountHomeService(ICompanyUserRepository companyUserRepository)
        {
            _companyUserRepository = companyUserRepository;
        }

        public async Task<bool> UpdateCompanyIpAddress(string abd2CId, string ipAddresses)
        {
            var company = await _companyUserRepository.GetCompanyByUserSingle(abd2CId);
            if (company != null)
            {
                if (string.IsNullOrWhiteSpace(company.ipaddresses) || !company.ipaddresses.Contains(ipAddresses, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    company.ipaddresses = ipAddresses;
                    await _companyUserRepository.UpdateCompany(company);
                }

                return true;
            }

            return false;
        }

        public async Task<VendorCompanyViewModel> GetCompanyByUserIdAsync(string abd2CId)
        {
            var company = await _companyUserRepository.GetCompanyByUserSingle(abd2CId);

            return VendorCompanyMapper.Map(company);
        }
    }
}