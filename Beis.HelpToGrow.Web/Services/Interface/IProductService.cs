namespace Beis.HelpToGrow.Web.Services.Interface
{
    using System.Threading.Tasks;

    using Beis.HelpToGrow.Web.Models;

    public interface IProductService
    {
        Task<bool> UpdateRedemptionUrl(RedemptionUrlViewModel redemptionUrlViewModel, string adb2CId);
 
        Task<bool> UpdateSku(SkuViewModel skuViewModel, string adb2CId);
        
        Task<bool> UpdateSummary(SummaryViewModel summaryViewModel, string adb2CId);
        
        Task<T> GetProduct<T>(long productId, string adb2CId);

        Task<string> GetProductName(long productId, string adb2CId);
    }
}