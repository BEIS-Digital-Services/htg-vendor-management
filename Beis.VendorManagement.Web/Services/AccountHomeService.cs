using AutoMapper;
using Beis.VendorManagement.Repositories.Interface;
using Beis.VendorManagement.Web.Models;
using Beis.VendorManagement.Web.Services.Interface;
using System.Threading.Tasks;

namespace Beis.VendorManagement.Web.Services
{
    public class AccountHomeService : IAccountHomeService
    {
        private readonly ICompanyUserRepository _companyUserRepository;
        private readonly IMapper _mapper;

        public AccountHomeService(ICompanyUserRepository companyUserRepository, IMapper mapper)
        {
            _companyUserRepository = companyUserRepository;
            _mapper = mapper;
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

            return _mapper.Map<VendorCompanyViewModel>(company);
        }
    }
}