namespace Beis.HelpToGrow.Core.Repositories.Interface
{
    using System.Threading.Tasks;

    using Beis.Htg.VendorSme.Database.Models;

    public interface IProductRepository
    {
        Task UpdateProduct(product product);

        Task<product> GetProductSingle(long id, string adb2CId);
    }
}