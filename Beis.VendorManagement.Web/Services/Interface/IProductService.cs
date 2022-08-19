namespace Beis.VendorManagement.Web.Services.Interface
{
    public interface IProductService
    {
        Task<bool> UpdateRedemptionUrl(RedemptionUrlViewModel redemptionUrlViewModel, string adb2CId);
 
        Task<bool> UpdateSku(SkuViewModel skuViewModel, string adb2CId);
        
        Task<bool> UpdateSummary(SummaryViewModel summaryViewModel, string adb2CId);
        
        Task<RedemptionUrlViewModel> GetRedemptionUrlDetails(long productId, string adb2CId);

        Task<SkuViewModel> GetSkuDetails(long productId, string adb2CId);

        Task<SummaryViewModel> GetSummaryDetails(long productId, string adb2CId);

        Task<ProductSubmitConfirmationViewModel> GetProductSubmitConfirmationDetails(long productId, string adb2CId);

        Task<string> GetProductName(long productId, string adb2CId);
    }
}