namespace Beis.HelpToGrow.Core.Repositories.Interface
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Beis.Htg.VendorSme.Database.Models;
    
    public interface ISettingsProductTypesRepository
    {
        Task<IList<settings_product_type>> GetSettingsProductTypes();

        Task<string> GetSettingsProductType(long productId);
    }
}