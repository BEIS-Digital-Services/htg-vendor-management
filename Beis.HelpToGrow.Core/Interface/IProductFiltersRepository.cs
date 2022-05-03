using Beis.Htg.VendorSme.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Beis.VendorManagement.Repositories.Interface
{
    public interface IProductFiltersRepository
    {
        Task<IEnumerable<product_filter>> GetProductFilters(long productId);

        Task AddProductFilters(IEnumerable<product_filter> productFilters);
        
        Task DeleteAllProductFilters(long productId, IEnumerable<long> filterTypes);
    }
}