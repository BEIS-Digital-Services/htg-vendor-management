using Beis.Htg.VendorSme.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Beis.VendorManagement.Repositories.Interface
{
    public interface ISettingsProductFiltersRepository
    {
        Task<IList<settings_product_filter>> GetSettingsProductFilters(long filterType);
    }
}