using Beis.VendorManagement.Web.Models;
using System.Threading.Tasks;

namespace Beis.VendorManagement.Web.Services.Interface
{
    public interface IProductService
    {
        Task<bool> UpdateRedemptionUrl(RedemptionUrlViewModel redemptionUrlViewModel, string adb2CId);
 
        Task<bool> UpdateSku(SkuViewModel skuViewModel, string adb2CId);
        
        Task<bool> UpdateSummary(SummaryViewModel summaryViewModel, string adb2CId);
        
        Task<T> GetProduct<T>(long productId, string adb2CId);

        Task<string> GetProductName(long productId, string adb2CId);
    }
}