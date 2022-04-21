namespace Beis.HelpToGrow.Core.Repositories.Interface
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Beis.Htg.VendorSme.Database.Models;

    public interface ISettingsProductFiltersCategoriesRepository
    {
        Task<IList<settings_product_filters_category>> GetSettingsProductFiltersCategories();
    }
}