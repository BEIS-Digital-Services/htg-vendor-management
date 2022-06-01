namespace Beis.VendorManagement.Repositories
{
    public class PricingRepository : IPricingRepository
    {
        private readonly HtgVendorSmeDbContext _context;

        public PricingRepository(HtgVendorSmeDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<product_price>> GetAllProductPricesForProductId(long id)
        {
            return await _context.product_prices.Where(x => x.productid == id).ToListAsync();
        }

        public async Task<product_price> GetAllProductPricesForProductIdAndPriceId(long id, long productPriceId)
        {
            return await _context.product_prices.Where(x => x.productid == id && x.product_price_id == productPriceId).FirstOrDefaultAsync();
        }

        public async Task<free_trial_end_action> GetFreeTrialEndAction(long id)
        {
            return await _context.free_trial_end_actions.Where(x => x.free_trial_end_action_id == id).FirstOrDefaultAsync();
        }

        public async Task<IList<product_price_base_description>> GetAllProductPriceBaseDescriptions()
        {
            return await _context.product_price_base_descriptions.ToListAsync();
        }

        public async Task<IEnumerable<product_price_base_metric_price>> GetAllProductBaseMetricPricesByProductPriceId(long productPriceId)
        {
            return await _context.product_price_base_metric_prices.Where(x => x.product_price_id == productPriceId).ToListAsync(); ;
        }

        public async Task<IList<product_price_secondary_description>> GetAllProductPriceSecondaryDescriptions()
        {
            return await _context.product_price_secondary_descriptions.ToListAsync();
        }

        public async Task<IEnumerable<product_price_secondary_metric>> GetAllProductSecondaryMetricPricesByProductPriceId(long productPriceId)
        {
            return await _context.product_price_secondary_metrics.Where(x => x.product_price_id == productPriceId).ToListAsync(); ;
        }

        public async Task<IEnumerable<user_discount>> GetUserDiscountByProductPriceId(long productPriceId)
        {
            var result = await _context.user_discounts.Where(x => x.product_price_id == productPriceId).ToListAsync();
            return result;
        }

        public async Task<IEnumerable<additional_cost>> GetAdditionalCostsByProductPriceId(long productPriceId)
        {
            return await _context.additional_costs.Where(x => x.product_price_id == productPriceId).ToListAsync();
        }

        public async Task<IList<additional_cost_desc>> GetAllAdditionalCostDescriptions()
        {
            return await _context.additional_cost_descs.ToListAsync();
        }
    }
}