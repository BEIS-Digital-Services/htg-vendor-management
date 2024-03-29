﻿namespace Beis.VendorManagement.Web.Handlers.ProductSubmit
{
    public class ProductSubmitReviewDetailsGetHandler : IRequestHandler<ProductSubmitReviewDetailsGetHandler.Context, Optional<ProductSubmitReviewDetailsViewModel>>
    {
        private readonly IProductRepository _productRepository;
        private readonly ISettingsProductCapabilitiesRepository _settingsProductCapabilitiesRepository;
        private readonly IProductCapabilitiesRepository _productCapabilitiesRepository;
        private readonly ISettingsProductFiltersCategoriesRepository _settingsProductFiltersCategoriesRepository;
        private readonly ISettingsProductFiltersRepository _settingsProductFiltersRepository;
        private readonly IProductFiltersRepository _productFiltersRepository;
        private readonly ProductLogoOptions _options;

        public ProductSubmitReviewDetailsGetHandler(
            IProductRepository productRepository,
            ISettingsProductCapabilitiesRepository settingsProductCapabilitiesRepository,
            IProductCapabilitiesRepository productCapabilitiesRepository,
            ISettingsProductFiltersCategoriesRepository settingsProductFiltersCategoriesRepository,
            ISettingsProductFiltersRepository settingsProductFiltersRepository,
            IProductFiltersRepository productFiltersRepository,
            IOptions<ProductLogoOptions> options)
        {
            _productRepository = productRepository;
            _settingsProductCapabilitiesRepository = settingsProductCapabilitiesRepository;
            _productCapabilitiesRepository = productCapabilitiesRepository;
            _settingsProductFiltersCategoriesRepository = settingsProductFiltersCategoriesRepository;
            _settingsProductFiltersRepository = settingsProductFiltersRepository;
            _productFiltersRepository = productFiltersRepository;
            _options = options.Value;
        }

        public async Task<Optional<ProductSubmitReviewDetailsViewModel>> Handle(Context request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetProductSingle(request.ProductId, request.Adb2CId);

            if (product == null)
            {
                return default;
            }

            var settingsProductCapabilities = (await _settingsProductCapabilitiesRepository.GetSettingsProductCapabilities())
                                              .Where(x => x.product_type == product.product_type).OrderBy(x => x.sort_order).ToList();
            var existingProductCapabilities = await _productCapabilitiesRepository.GetProductCapabilitiesFilters(request.ProductId);

            var items = settingsProductCapabilities.Select(x => new SelectListItem
            {
                Text = x.capability_name,
                Value = x.capability_id.ToString()
            });

            //Assign existing filters
            var productCapabilities = new List<string>();
            var lstItems = items.ToList();
            foreach (var existingProductCapability in existingProductCapabilities)
            {
                productCapabilities.AddRange(from lstItem in lstItems where lstItem.Value == existingProductCapability.capability_id.ToString() select lstItem.Text);
            }

            if (!string.IsNullOrWhiteSpace(product.draft_other_capabilities))
            {
                productCapabilities.Add(product.draft_other_capabilities);
            }

            var productLogo = string.Empty;
            if (!string.IsNullOrWhiteSpace(product.product_logo))
            {
                productLogo = 
                    $"{request.Scheme}://{request.Host}{_options.LogoPath}{product.product_logo.Substring(product.product_logo.LastIndexOf(@"\", StringComparison.OrdinalIgnoreCase) + 1)}";
            }

            var productSubmitReviewDetails = new ProductSubmitReviewDetailsViewModel
            {
                ProductId = product.product_id,
                DraftProductDescription = product.draft_product_description,
                RedemptionUrl = product.redemption_url,
                Adb2CId = request.Adb2CId,
                ProductName = product.product_name,
                DraftOtherCompatibility = product.draft_other_compatability,
                DraftReviewUrl = product.draft_review_url,
                ProductLogo = productLogo,
                ProductCapabilities = productCapabilities
            };

            await AddOtherItems(productSubmitReviewDetails, request.ProductId);

            productSubmitReviewDetails.ContentKey = $"{AnalyticConstants.ProductSubmitReviewDetails}{productSubmitReviewDetails.ProductName}";
            return productSubmitReviewDetails;
        }

        private async Task AddOtherItems(ProductSubmitReviewDetailsViewModel productDetails, long productId)
        {
            var filterTypes = new List<long> {(int)ProductFilterCategories.Support,
                                                (int)ProductFilterCategories.Training,
                                                (int)ProductFilterCategories.Platform };

            var settingsProductFiltersCategories = (await _settingsProductFiltersCategoriesRepository.GetSettingsProductFiltersCategories())
                                                    .Where(x => filterTypes.Contains(x.id)).OrderBy(x => x.id).ToList();
            var settingsProductFilters = await _settingsProductFiltersRepository.GetSettingsProductFilters(0);
            var productFilters = await _productFiltersRepository.GetProductFilters(productId);

            foreach (var settingsProductFiltersCategoryId in settingsProductFiltersCategories.Select(s => s.id))
            {
                var temp = settingsProductFilters.Where(x => x.filter_type == settingsProductFiltersCategoryId).ToList();
                var items = temp.Select(x => new SelectListItem { Text = x.filter_name, Value = x.filter_id.ToString() });

                var lstItems = items.ToList();
                foreach (var productFilter in productFilters)
                {
                    foreach (var filterName in lstItems.Where(lstItem => lstItem.Value == productFilter.filter_id.ToString()).Select(i => i.Text))
                    {
                        switch (settingsProductFiltersCategoryId)
                        {
                            case (int)ProductFilterCategories.Support:
                                productDetails.SupportItems ??= new List<string>();
                                productDetails.SupportItems.Add(filterName);
                                break;
                            case (int)ProductFilterCategories.Training:
                                productDetails.TrainingItems ??= new List<string>();
                                productDetails.TrainingItems.Add(filterName);
                                break;
                            case (int)ProductFilterCategories.Platform:
                                productDetails.PlatformItems ??= new List<string>();
                                productDetails.PlatformItems.Add(filterName);
                                break;
                        }
                    }
                }
            }
        }

        public struct Context : IRequest<Optional<ProductSubmitReviewDetailsViewModel>>
        {
            public long ProductId { get; set; }

            public string Adb2CId { get; set; }

            public string Host { get; set; }

            public string Scheme { get; set; }
        }
    }
}