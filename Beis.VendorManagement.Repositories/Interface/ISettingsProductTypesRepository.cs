using Beis.Htg.VendorSme.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Beis.VendorManagement.Repositories.Interface
{
    public interface ISettingsProductTypesRepository
    {
        Task<IList<settings_product_type>> GetSettingsProductTypes();

        Task<string> GetSettingsProductType(long productId);
    }
}