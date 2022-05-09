using Beis.Htg.VendorSme.Database.Models;
using System.Threading.Tasks;

namespace Beis.VendorManagement.Repositories.Interface
{
    public interface IVendorCompanyRepository
    {
        Task<vendor_company> GetVendorCompanySingle(long id);
    }
}