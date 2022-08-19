using Beis.VendorManagement.Web.Mappers;

namespace Beis.VendorManagement.Web.Services
{
    public class VendorCompanyService : IVendorCompanyService
    {
        private readonly IVendorCompanyRepository _vendorCompanyRepository;
        public VendorCompanyService(IVendorCompanyRepository vendorCompanyRepository)
        {
            _vendorCompanyRepository = vendorCompanyRepository;
        }

        public async Task<VendorCompanyViewModel> GetVendorCompany(long id)
        {
            return VendorCompanyMapper.Map(await _vendorCompanyRepository.GetVendorCompanySingle(id));
        }
    }
}