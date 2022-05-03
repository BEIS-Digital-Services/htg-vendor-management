using Beis.Htg.VendorSme.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Beis.VendorManagement.Repositories.Interface
{
    public interface IProductCapabilitiesRepository
    {
        Task<IEnumerable<product_capability>> GetProductCapabilitiesFilters(long productId = 0);

        Task AddProductCapabilitiesFilters(IEnumerable<product_capability> product_capabilities);
        
        Task DeleteAllProductCapabilitiesFilters(long productId, long productType);
    }
}