namespace Beis.VendorManagement.Repositories.Interface
{
    public interface IPricingRepository
    {
        Task<IEnumerable<product_price>> GetAllProductPricesForProductId(long id);

        Task<product_price> GetAllProductPricesForProductIdAndPriceId(long id, long productPriceId);
        
        Task<IList<product_price_base_description>> GetAllProductPriceBaseDescriptions();
        
        Task<IEnumerable<product_price_base_metric_price>> GetAllProductBaseMetricPricesByProductPriceId(long productPriceId);
        
        Task<IList<product_price_secondary_description>> GetAllProductPriceSecondaryDescriptions();
        
        Task<IEnumerable<product_price_secondary_metric>> GetAllProductSecondaryMetricPricesByProductPriceId(long productPriceId);
        
        Task<IEnumerable<user_discount>> GetUserDiscountByProductPriceId(long productPriceId);
        
        Task<IEnumerable<additional_cost>> GetAdditionalCostsByProductPriceId(long productPriceId);
        
        Task<IList<additional_cost_desc>> GetAllAdditionalCostDescriptions();
        
        Task<free_trial_end_action> GetFreeTrialEndAction(long id);
    }
}