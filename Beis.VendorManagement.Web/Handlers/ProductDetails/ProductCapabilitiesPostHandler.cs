namespace Beis.VendorManagement.Web.Handlers.ProductDetails
{
    public class ProductCapabilitiesPostHandler : IRequestHandler<ProductCapabilitiesPostHandler.Context>
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductCapabilitiesRepository _productCapabilitiesRepository;
        private readonly IMapper _mapper;

        public ProductCapabilitiesPostHandler(
            IProductRepository productRepository,
            IProductCapabilitiesRepository productCapabilitiesRepository,
            IMapper mapper)
        {
            _productRepository = productRepository;
            _productCapabilitiesRepository = productCapabilitiesRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(Context request, CancellationToken cancellationToken)
        {
            var productFilters = new List<ProductCapabilitiesModel>();

            foreach (var productCapabilitiesFilter in request.ProductCapabilities.SettingsProductCapabilitiesList.Where(x => x.Selected))
            {
                var filter = new ProductCapabilitiesModel
                {
                    CapabilityId = Convert.ToInt64(productCapabilitiesFilter.Value),
                    ProductId = request.ProductId
                };
                productFilters.Add(filter);
            }

            var approvedFilters = (await _productCapabilitiesRepository.GetProductCapabilitiesFilters(request.ProductId)).Where(af => af.draft_capability != "Y").ToList();
            
            // if the chosen ones are different than approved
            var isDifferent = productFilters.Count != approvedFilters.Count ||
                               productFilters.Any(o => !approvedFilters.Any(w => w.capability_id == o.CapabilityId));
            if (isDifferent)
            {
                foreach (var productFilter in productFilters)
                    productFilter.DraftCapability = "Y";
            }

            await _productCapabilitiesRepository.DeleteAllProductCapabilitiesFilters(request.ProductId, request.ProductTypeId);
            if (productFilters.ToList().Count > 0)
            {
                await _productCapabilitiesRepository.AddProductCapabilitiesFilters(_mapper.Map<IEnumerable<product_capability>>(productFilters));
            }

            var product = await _productRepository.GetProductSingle(request.ProductId, request.Adb2CId);

            if (product != null && request.ProductCapabilities.DraftAdditionalCapabilities != null &&
                !request.ProductCapabilities.DraftAdditionalCapabilities.Equals(product.draft_other_capabilities,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                product.draft_other_capabilities = request.ProductCapabilities.DraftAdditionalCapabilities;
                await _productRepository.UpdateProduct(product);
            }

            return Unit.Value;
        }

        public struct Context : IRequest
        {
            public long ProductId { get; set; }

            public string Adb2CId { get; set; }

            public long ProductTypeId { get; set; }

            public ProductCapabilitiesViewModel ProductCapabilities { get; set; }
        }
    }
}