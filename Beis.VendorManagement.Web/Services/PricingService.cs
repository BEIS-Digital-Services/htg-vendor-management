namespace Beis.VendorManagement.Web.Services
{
    public class PricingService : IPricingService
    {
        private readonly IPricingRepository _pricingRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public PricingService(IMapper mapper, IProductRepository productRepository, IPricingRepository pricingRepository)
        {
            _mapper = mapper;
            _productRepository = productRepository;
            _pricingRepository = pricingRepository;
        }

        public async Task<ProductPriceDetailsViewModel> GetAllProductPrices(long productId, string adb2CId)
        {
            var product = await _productRepository.GetProductSingle(productId, adb2CId);
            var productPrices = _mapper.Map<IList<ProductPriceViewModel>>(await _pricingRepository.GetAllProductPricesForProductId(productId));

            foreach (var item in productPrices)
            {
                item.VoucherUrl = product.redemption_url;
            }

            return new ProductPriceDetailsViewModel
            {
                ProductId = productId,
                Adb2CId = adb2CId,
                ProductName = product.product_name,
                ProductPrices = productPrices
            };
        }

        public async Task<AdditionalDiscountsViewModel> GetAdditionalDiscountsForPriceId(long productId, string adb2CId, long productPriceId)
        {
            var productPrice = await _pricingRepository.GetAllProductPricesForProductIdAndPriceId(productId, productPriceId);
            var additionalDiscounts = _mapper.Map<AdditionalDiscountsViewModel>(productPrice);

            if (additionalDiscounts == null) return default;

            additionalDiscounts.HasPricing = true;
            additionalDiscounts.Adb2CId = adb2CId;
            additionalDiscounts.ProductName = (await _productRepository.GetProductSingle(productId, adb2CId)).product_name;
            additionalDiscounts.UserDiscounts = await GetUserDiscountByProductPriceId(productPriceId);
            return additionalDiscounts;
        }

        public async Task<IEnumerable<UserDiscountViewModel>> GetUserDiscountByProductPriceId(long productPriceId)
        {
            var userDiscount = await _pricingRepository.GetUserDiscountByProductPriceId(productPriceId);
            var userDiscountVm = _mapper.Map<IEnumerable<UserDiscountViewModel>>(userDiscount);
            return userDiscountVm;
        }


        public async Task<MetricDetailsViewModel> GetMetricDetails(int productId, string adb2CId, long productPriceId)
        {
            var primaryMetricPricing = await _pricingRepository.GetAllProductBaseMetricPricesByProductPriceId(productPriceId);
            var secondaryMetricPricing = await _pricingRepository.GetAllProductSecondaryMetricPricesByProductPriceId(productPriceId);

            var baseDescriptions = await _pricingRepository.GetAllProductPriceBaseDescriptions();
            var secondaryDescriptions = await _pricingRepository.GetAllProductPriceSecondaryDescriptions();

            var metricDetails = new MetricDetailsViewModel
            {
                ProductPriceId = productPriceId,
                ProductId = productId,
                Adb2CId = adb2CId,
                PrimaryMetricDetails = new List<PrimaryMetricDetailsViewModel>(),
                SecondaryMetricDetails = new List<SecondaryMetricDetailsViewModel>()
            };

            foreach (var item in primaryMetricPricing)
            {
                var primaryMetricDetails = new PrimaryMetricDetailsViewModel
                {
                    Amount = item.product_price_amount,
                    PricePercentage = item.product_price_percentage,
                    NumberOfUsers = item.product_price_no_users,
                    Description = baseDescriptions.First(x => x.product_price_base_description_id == item.product_price_base_description_id).product_price_basedescription
                };
                metricDetails.PrimaryMetricDetails.Add(primaryMetricDetails);
            }

            foreach (var item in secondaryMetricPricing)
            {
                var secondaryMetricDetails = new SecondaryMetricDetailsViewModel
                {
                    MetricNumber = item.metric_no,
                    MetricUnit = item.metric_unit,
                    Description = secondaryDescriptions.First(x => x.product_price_sec_description_id == item.product_price_sec_description_id).product_price_sec_description
                };
                metricDetails.SecondaryMetricDetails.Add(secondaryMetricDetails);
            }

            var product = await _productRepository.GetProductSingle(productId, adb2CId);
            metricDetails.ProductName = product.product_name;

            return metricDetails;
        }

        public async Task<MinimumCommitmentViewModel> GetMinimumCommitment(long productId, string adb2CId, long productPriceId)
        {
            var productPrice = await _pricingRepository.GetAllProductPricesForProductIdAndPriceId(productId, productPriceId);
            var minimumCommitmentViewModel = _mapper.Map<MinimumCommitmentViewModel>(productPrice);

            if (productPrice == null) return default;

            var product = await _productRepository.GetProductSingle(productId, adb2CId);
            minimumCommitmentViewModel.ProductName = product.product_name;
            minimumCommitmentViewModel.Adb2CId = adb2CId;
            return minimumCommitmentViewModel;
        }

        public async Task<FreeTrialViewModel> GetFreeTrial(long productId, string adb2CId, long productPriceId)
        {
            var product = await _productRepository.GetProductSingle(productId, adb2CId);
            var freeTrialViewModel = new FreeTrialViewModel
            {
                ProductName = product.product_name,
                ProductId = product.product_id,
                Adb2CId = adb2CId,
                ProductPriceId = productPriceId
            };

            var productPrice = await _pricingRepository.GetAllProductPricesForProductIdAndPriceId(productId, productPriceId);
            if (productPrice == null) return freeTrialViewModel;

            freeTrialViewModel.FreeTrialFlag = productPrice.free_trial_flag;
            freeTrialViewModel.FreeTrialPaymentUpfront = productPrice.free_trial_payment_upfront;
            freeTrialViewModel.FreeTrialTermNo = productPrice.free_trial_term_no;
            freeTrialViewModel.FreeTrialTermUnit = productPrice.free_trial_term_unit;

            var freeTrialEndAction = await _pricingRepository.GetFreeTrialEndAction(
                Convert.ToInt64(productPrice.free_trial_end_action_id));

            if (freeTrialEndAction == null) return freeTrialViewModel;

            freeTrialViewModel.FreeTrialEndActionDescription = freeTrialEndAction.free_trial_end_action_desc;

            return freeTrialViewModel;
        }

        public async Task<DiscountPeriodViewModel> GetDiscountPeriod(long productId, string adb2CId, long productPriceId)
        {
            var productPrice = await _pricingRepository.GetAllProductPricesForProductIdAndPriceId(productId, productPriceId);
            var discountPeriodViewModel = _mapper.Map<DiscountPeriodViewModel>(productPrice);
            discountPeriodViewModel.Adb2CId = adb2CId;
            var product = await _productRepository.GetProductSingle(productId, adb2CId);
            discountPeriodViewModel.ProductName = product.product_name;
            return discountPeriodViewModel;
        }

        public async Task<AdditionalCostsViewModel> GetAdditionalCosts(long productId, string adb2CId, long productPriceId)
        {
            var additionalCosts = await _pricingRepository.GetAdditionalCostsByProductPriceId(productPriceId);
            var additionalCostDescriptions = await _pricingRepository.GetAllAdditionalCostDescriptions();

            var additionalCostsViewModel = new AdditionalCostsViewModel
            {
                ProductId = productId,
                ProductPriceId = productPriceId,
                Adb2CId = adb2CId
            };

            foreach (var item in additionalCosts)
            {
                var additionalCostDetail = new AdditionalCostDetail
                {
                    IsMandatory = item.additional_cost_mandatory_flag,
                    Type = additionalCostDescriptions.FirstOrDefault(x => x.additional_cost_desc_id == item.additional_cost_desc_id)?.additional_costDesc,
                    CostAndFrequency = $"£{item.additionalCost} {item.additional_cost_freq}"
                };
                additionalCostsViewModel.AdditionalCosts.Add(additionalCostDetail);
            }

            var product = await _productRepository.GetProductSingle(productId, adb2CId);
            additionalCostsViewModel.ProductName = product.product_name;

            return additionalCostsViewModel;
        }
    }
}