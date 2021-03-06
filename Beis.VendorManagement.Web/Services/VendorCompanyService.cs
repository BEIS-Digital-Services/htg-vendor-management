namespace Beis.VendorManagement.Web.Services
{
    public class VendorCompanyService : IVendorCompanyService
    {
        private readonly IVendorCompanyRepository _vendorCompanyRepository;
        private readonly IMapper _mapper;
        public VendorCompanyService(IMapper mapper, IVendorCompanyRepository vendorCompanyRepository)
        {
            _mapper = mapper;
            _vendorCompanyRepository = vendorCompanyRepository;
        }

        public async Task<VendorCompanyViewModel> GetVendorCompany(long id)
        {
            return _mapper.Map<VendorCompanyViewModel>(await _vendorCompanyRepository.GetVendorCompanySingle(id));
        }
    }
}