namespace Beis.VendorManagement.Web.Services
{
    public class ActivateAccountService : IActivateAccountService
    {
        private readonly ICompanyUserRepository _companyUserRepository;
        private readonly IMapper _mapper;

        public ActivateAccountService(ICompanyUserRepository companyUserRepository, IMapper mapper)
        {
            _companyUserRepository = companyUserRepository;
            _mapper = mapper;
        }

        public async Task<VendorCompanyViewModel> GetCompanyByUserIdAsync(long userId)
        {
            var company = await _companyUserRepository.GetCompanyByUserSingle(userId);

            return _mapper.Map<VendorCompanyViewModel>(company);
        }

        public async Task<VendorCompanyUserViewModel> GetUserById(long userId)
        {
            var user = await _companyUserRepository.GetUserByIdSingle(userId);

            return _mapper.Map<VendorCompanyUserViewModel>(user);
        }

        public async Task<VendorCompanyUserViewModel> GetUserByAccessLink(string accessLink)
        {
            var user = await _companyUserRepository.GetUserByAccessLink(accessLink);
            return _mapper.Map<VendorCompanyUserViewModel>(user);
        }
    }
}