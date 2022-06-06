namespace Beis.VendorManagement.Web.Handlers.Product
{
    public class ProductLogoPostHandler : IRequestHandler<ProductLogoPostHandler.Context>
    {
        private readonly IProductRepository _productRepository;

        public ProductLogoPostHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Unit> Handle(Context request, CancellationToken cancellationToken)
        {
            var newFileName = $"{Path.GetFileNameWithoutExtension(request.File.FileName)}{DateTime.Now:ddMMyyHHmmss}{Path.GetExtension(request.File.FileName)}";
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", newFileName);

            await using (var stream = new FileStream(Path.Combine(path), FileMode.Create))
            {
                await request.File.CopyToAsync(stream, cancellationToken);
            }

            var product = await _productRepository.GetProductSingle(request.ProductId, request.Adb2CId);
            if (product != null)
            {
                product.product_logo = newFileName;
                await _productRepository.UpdateProduct(product);
            }

            return Unit.Value;
        }

        public struct Context : IRequest
        {
            public long ProductId { get; set; }

            public string Adb2CId { get; set; }

            public IFormFile File { get; set; }
        }
    }
}