using Beis.VendorManagement.Web.Mappers;

namespace Beis.VendorManagement.Web.Services
{
    public class ActivateAccountService : IActivateAccountService
    {
        private readonly ICompanyUserRepository _companyUserRepository;

        public ActivateAccountService(ICompanyUserRepository companyUserRepository)
        {
            _companyUserRepository = companyUserRepository;
        }

        public async Task<VendorCompanyViewModel> GetCompanyByUserIdAsync(long userId)
        {
            var company = await _companyUserRepository.GetCompanyByUserSingle(userId);
            return VendorCompanyMapper.Map(company);
        }

        public async Task<VendorCompanyUserViewModel> GetUserById(long userId)
        {
            var user = await _companyUserRepository.GetUserByIdSingle(userId);
            return VendorCompanyUserMapper.Map(user);
        }

        public async Task<VendorCompanyUserViewModel> GetUserByAccessLink(string accessLink)
        {
            var user = await _companyUserRepository.GetUserByAccessLink(accessLink);
            return VendorCompanyUserMapper.Map(user);
        }
    }
}