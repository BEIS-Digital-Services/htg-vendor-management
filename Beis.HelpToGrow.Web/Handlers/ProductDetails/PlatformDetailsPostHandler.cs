using AutoMapper;
using Beis.Htg.VendorSme.Database.Models;
using Beis.VendorManagement.Repositories.Interface;
using Beis.VendorManagement.Web.Models;
using Beis.VendorManagement.Web.Models.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Beis.VendorManagement.Web.Handlers.ProductDetails
{
    public class PlatformDetailsPostHandler : IRequestHandler<PlatformDetailsPostHandler.Context>
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductFiltersRepository _productFiltersRepository;
        private readonly IMapper _mapper;

        public PlatformDetailsPostHandler(
            IProductRepository productRepository,
            IProductFiltersRepository productFiltersRepository,
            IMapper mapper)
        {
            _productRepository = productRepository;
            _productFiltersRepository = productFiltersRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(Context request, CancellationToken cancellationToken)
        {
            var productFilters = new List<ProductFiltersModel>();

            foreach (var productfilter in request.PlatformDetails.SettingsProductFiltersCategory.SettingsProductFilters.Where(x => x.Selected == true))
            {
                var filter = new ProductFiltersModel
                {
                    FilterId = Convert.ToInt64(productfilter.Value),
                    ProductId = request.ProductId
                };
                productFilters.Add(filter);
            }

            var approvedFilters = await _productFiltersRepository.GetProductFilters(request.ProductId);
            approvedFilters = approvedFilters.Where(af => !af.draft_filter).ToList();
            // if the chosen ones are different than approved
            var isDifferent = (productFilters.Count != approvedFilters.Count()) ||
                               productFilters.Any(o => !approvedFilters.Any(w => w.filter_id == o.FilterId));
            if (isDifferent)
            {
                foreach (var productFilter in productFilters)
                    productFilter.DraftFilter = true;
            }

            var filterTypes = new List<long> { (int)ProductFilterCategories.Platform };

            await _productFiltersRepository.DeleteAllProductFilters(request.ProductId, filterTypes);
            if (productFilters.ToList().Count > 0)
            {
                await _productFiltersRepository.AddProductFilters(_mapper.Map<IEnumerable<product_filter>>(productFilters));
            }

            var product = await _productRepository.GetProductSingle(request.ProductId, request.Adb2CId);
            if (product != null)
            {
                product.draft_other_compatability = request.PlatformDetails.OtherCompatibility;
                product.draft_review_url = request.PlatformDetails.ReviewUrl;
                await _productRepository.UpdateProduct(product);
            }
            
            return Unit.Value;
        }

        public struct Context : IRequest
        {
            public long ProductId { get; set; }

            public string Adb2CId { get; set; }

            public PlatformDetailsViewModel PlatformDetails { get; set; }
        }
    }
}