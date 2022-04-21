namespace Beis.HelpToGrow.Web.Services.Interface
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    
    using Beis.HelpToGrow.Web.Models;
    using Beis.HelpToGrow.Web.Models.Pricing;
    
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