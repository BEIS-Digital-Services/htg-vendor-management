namespace Beis.HelpToGrow.Core.Repositories.Interface
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Beis.Htg.VendorSme.Database.Models;

    public interface IProductFiltersRepository
    {
        Task<IEnumerable<product_filter>> GetProductFilters(long productId);

        Task AddProductFilters(IEnumerable<product_filter> productFilters);
        
        Task DeleteAllProductFilters(long productId, IEnumerable<long> filterTypes);
    }
}