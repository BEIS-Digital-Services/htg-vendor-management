using Beis.Htg.VendorSme.Database.Models;
using System.Threading.Tasks;

namespace Beis.VendorManagement.Repositories.Interface
{
    public interface IProductRepository
    {
        Task UpdateProduct(product product);

        Task<product> GetProductSingle(long id, string adb2CId);
    }
}