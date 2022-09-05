namespace Beis.VendorManagement.Web.Handlers.ProductDetails
{
    public class ProductSupportGetHandler : IRequestHandler<ProductSupportGetHandler.Context, Optional<ProductSupportViewModel>>
    {
        private readonly IProductRepository _productRepository;
        private readonly ISettingsProductFiltersCategoriesRepository _settingsProductFiltersCategoriesRepository;
        private readonly ISettingsProductFiltersRepository _settingsProductFiltersRepository;
        private readonly IProductFiltersRepository _productFiltersRepository;

        public ProductSupportGetHandler(
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

        public async Task<Optional<ProductSupportViewModel>> Handle(Context request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetProductSingle(request.ProductId, request.Adb2CId);

            if (product == null)
            {
                return default;
            }

            var filterTypes = new List<long> { (int)ProductFilterCategories.Support, (int)ProductFilterCategories.Training };
            var settingsProductFiltersCategories = (await _settingsProductFiltersCategoriesRepository.GetSettingsProductFiltersCategories())
                                                    .Where(x => filterTypes.Contains(x.id)).OrderBy(x => x.id).ToList();

            var settingsProductFilters = await _settingsProductFiltersRepository.GetSettingsProductFilters(0);

            var productSupportViewModel = new ProductSupportViewModel
            {
                ProductId = product.product_id,
                ProductName = product.product_name,
                ProductStatus = (ProductStatus)product.status,
                Adb2CId = request.Adb2CId
            };

            var productFilters = await _productFiltersRepository.GetProductFilters(request.ProductId);
            productSupportViewModel.SettingsProductFiltersCategories = settingsProductFiltersCategories.GetSettingsProductFilterCategory(settingsProductFilters, productFilters);
            productSupportViewModel.ContentKey = $"{AnalyticConstants.ProductSupport}{productSupportViewModel.ProductName}";
            return productSupportViewModel;
        }

        public struct Context : IRequest<Optional<ProductSupportViewModel>>
        {
            public long ProductId { get; set; }

            public string Adb2CId { get; set; }
        }
    }
}