namespace Beis.HelpToGrow.Core.Repositories.Interface
{
    using System.Threading.Tasks;

    using Beis.Htg.VendorSme.Database.Models;

    public interface IVendorCompanyRepository
    {
        Task<vendor_company> GetVendorCompanySingle(long id);
    }
}