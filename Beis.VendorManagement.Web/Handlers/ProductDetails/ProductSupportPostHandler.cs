﻿namespace Beis.VendorManagement.Web.Handlers.ProductDetails
{
    public class ProductSupportPostHandler : IRequestHandler<ProductSupportPostHandler.Context>
    {
        private readonly IProductFiltersRepository _productFiltersRepository;

        public ProductSupportPostHandler(IProductFiltersRepository productFiltersRepository)
        {
            _productFiltersRepository = productFiltersRepository;
        }

        public async Task<Unit> Handle(Context request, CancellationToken cancellationToken)
        {
            var productFilters = new List<ProductFiltersModel>();

            foreach (var item in request.ProductSupport.SettingsProductFiltersCategories)
            {
                foreach (var productfilter in item.SettingsProductFilters.Where(x => x.Selected))
                {
                    var filter = new ProductFiltersModel
                    {
                        FilterId = Convert.ToInt64(productfilter.Value),
                        ProductId = request.ProductId
                    };
                    productFilters.Add(filter);
                }
            }

            var approvedFilters = (await _productFiltersRepository.GetProductFilters(request.ProductId)).Where(af => !af.draft_filter).ToList();
            // if the chosen ones are different than approved
            var isDifferent = (productFilters.Count != approvedFilters.Count) ||
                               productFilters.Any(o => !approvedFilters.Any(w => w.filter_id == o.FilterId));
            if (isDifferent)
            {
                foreach (var productFilter in productFilters)
                    productFilter.DraftFilter = true;
            }

            var filterTypes = new List<long> { (int)ProductFilterCategories.Support, (int)ProductFilterCategories.Training };

            await _productFiltersRepository.DeleteAllProductFilters(request.ProductId, filterTypes);
            if (productFilters.ToList().Count > 0)
            {
                await _productFiltersRepository.AddProductFilters(ProductMapper.MapProductFilterDetails(productFilters));
            }

            return Unit.Value;
        }

        public struct Context : IRequest
        {
            public long ProductId { get; set; }

            public ProductSupportViewModel ProductSupport { get; set; }
        }
    }
}