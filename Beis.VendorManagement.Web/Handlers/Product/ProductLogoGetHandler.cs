﻿namespace Beis.VendorManagement.Web.Handlers.Product
{
    public class ProductLogoGetHandler : IRequestHandler<ProductLogoGetHandler.Context, Optional<ProductLogoViewModel>>
    {
        private readonly IProductRepository _productRepository;
        private readonly ProductLogoOptions _options;

        public ProductLogoGetHandler(IProductRepository productRepository, IOptions<ProductLogoOptions> options)
        {
            _productRepository = productRepository;
            _options = options.Value;
        }

        public async Task<Optional<ProductLogoViewModel>> Handle(Context request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetProductSingle(request.ProductId, request.Adb2CId);
            if (product == null)
            {
                return default;
            }

            var productLogoViewModel = new ProductLogoViewModel
            {
                Adb2CId = request.Adb2CId,
                ProductId = product.product_id,
                ProductName = product.product_name
            };

            if (!string.IsNullOrWhiteSpace(product.product_logo))
            {
                productLogoViewModel.ProductLogo = 
                    $"{request.Scheme}://{request.Host}{_options.LogoPath}{product.product_logo.Substring(product.product_logo.LastIndexOf(@"\", StringComparison.OrdinalIgnoreCase) + 1)}";
            }

            productLogoViewModel.ContentKey = $"{AnalyticConstants.ProductLogo}{productLogoViewModel.ProductName}";
            return productLogoViewModel;
        }

        public struct Context : IRequest<Optional<ProductLogoViewModel>>
        {
            public long ProductId { get; set; }

            public string Adb2CId { get; set; }

            public string Host { get; set; }

            public string Scheme { get; set; }
        }
    }
}