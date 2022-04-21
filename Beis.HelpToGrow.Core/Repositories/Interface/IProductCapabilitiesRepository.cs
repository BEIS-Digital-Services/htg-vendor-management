namespace Beis.HelpToGrow.Core.Repositories.Interface
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Beis.Htg.VendorSme.Database.Models;

    public interface IProductCapabilitiesRepository
    {
        Task<IEnumerable<product_capability>> GetProductCapabilitiesFilters(long productId = 0);

        Task AddProductCapabilitiesFilters(IEnumerable<product_capability> product_capabilities);
        
        Task DeleteAllProductCapabilitiesFilters(long productId, long productType);
    }
}