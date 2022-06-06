namespace Beis.VendorManagement.Web.Services.Interface
{
    public interface IPricingService
    {
        Task<ProductPriceDetailsViewModel> GetAllProductPrices(long productId, string adb2CId);
        
        Task<MetricDetailsViewModel> GetMetricDetails(int productId, string adb2CId, long productPriceId);

        Task<MinimumCommitmentViewModel> GetMinimumCommitment(long id, string adb2CId, long productPriceId);

        Task<FreeTrialViewModel> GetFreeTrial(long id, string adb2CId, long productPriceId);

        Task<DiscountPeriodViewModel> GetDiscountPeriod(long id, string adb2CId, long productPriceId);

        Task<AdditionalCostsViewModel> GetAdditionalCosts(long productId, string adb2CId, long productPriceId);
        
        Task<AdditionalDiscountsViewModel> GetAdditionalDiscountsForPriceId(long id, string adb2CId, long productPriceId);
    }
}