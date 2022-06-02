using Beis.VendorManagement.Web.Extensions;

namespace Beis.VendorManagement.Web.Handlers.ProductDetails
{
    public class PlatformDetailsGetHandler : IRequestHandler<PlatformDetailsGetHandler.Context, Optional<PlatformDetailsViewModel>>
    {
        private readonly IProductRepository _productRepository;
        private readonly ISettingsProductFiltersCategoriesRepository _settingsProductFiltersCategoriesRepository;
        private readonly ISettingsProductFiltersRepository _settingsProductFiltersRepository;
        private readonly IProductFiltersRepository _productFiltersRepository;

        public PlatformDetailsGetHandler(
            IProductRepository productRepository,
            ISettingsProductFiltersCategoriesRepository settingsProductFiltersCategoriesRepository,
            ISettingsProductFiltersRepository settingsProductFiltersRepository,
            IProductFiltersRepository productFiltersRepository)
        {
            _productRepository = productRepository;
            _settingsProductFiltersCategoriesRepository = settingsProductFiltersCategoriesRepository;
            _settingsProductFiltersRepository = settingsProductFiltersRepository;
            _productFiltersRepository = productFiltersRepository;
        }

        public async Task<Optional<PlatformDetailsViewModel>> Handle(Context request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetProductSingle(request.ProductId, request.Adb2CId);

            if (product == null)
            {
                return default;
            }

            var filterTypes = new List<long> { (int)ProductFilterCategories.Platform };
            var settingsProductFiltersCategories = (await _settingsProductFiltersCategoriesRepository.GetSettingsProductFiltersCategories())
                                                    .Where(x => filterTypes.Contains(x.id)).OrderBy(x => x.id).ToList();
            var settingsProductFilters = await _settingsProductFiltersRepository.GetSettingsProductFilters(0);

            var platformDetailsVm = new PlatformDetailsViewModel
            {
                ProductId = product.product_id,
                ProductName = product.product_name,
                OtherCompatibility = !string.IsNullOrWhiteSpace(product.draft_other_compatability) ?
                                    product.draft_other_compatability : product.other_compatability,
                ReviewUrl = !string.IsNullOrEmpty(product.draft_review_url) ?
                                    product.draft_review_url : product.review_url,
                ProductStatus = (ProductStatus)product.status,
                Adb2CId = request.Adb2CId
            };

            var productFilters = await _productFiltersRepository.GetProductFilters(request.ProductId);
            platformDetailsVm.SettingsProductFiltersCategory = settingsProductFiltersCategories.GetSettingsProductFilterCategory(settingsProductFilters, productFilters).First();
            platformDetailsVm.ContentKey = $"{AnalyticConstants.ProductPlatformDetails}{platformDetailsVm.ProductName}";
            return platformDetailsVm;
        }

        public struct Context : IRequest<Optional<PlatformDetailsViewModel>>
        {
            public long ProductId { get; set; }

            public string Adb2CId { get; set; }
        }
    }
}