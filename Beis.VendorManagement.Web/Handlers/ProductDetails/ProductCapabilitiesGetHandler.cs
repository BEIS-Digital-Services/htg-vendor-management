using Beis.VendorManagement.Repositories.Interface;
using Beis.VendorManagement.Web.Models;
using Beis.VendorManagement.Web.Models.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Beis.VendorManagement.Web.Handlers.ProductDetails
{
    public class ProductCapabilitiesGetHandler : IRequestHandler<ProductCapabilitiesGetHandler.Context, Optional<ProductCapabilitiesViewModel>>
    {
        private readonly IProductRepository _productRepository;
        private readonly ISettingsProductCapabilitiesRepository _settingsProductCapabilitiesRepository;
        private readonly IProductCapabilitiesRepository _productCapabilitiesRepository;

        public ProductCapabilitiesGetHandler(IProductRepository productRepository, 
            ISettingsProductCapabilitiesRepository settingsProductCapabilitiesRepository, 
            IProductCapabilitiesRepository productCapabilitiesRepository)
        {
            _productRepository = productRepository;
            _settingsProductCapabilitiesRepository = settingsProductCapabilitiesRepository;
            _productCapabilitiesRepository = productCapabilitiesRepository;
        }

        public async Task<Optional<ProductCapabilitiesViewModel>> Handle(Context request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetProductSingle(request.ProductId, request.Adb2CId);

            if (product == null)
            {
                return default;
            }

            var settingsProductCapabilities = await _settingsProductCapabilitiesRepository.GetSettingsProductCapabilities();
            settingsProductCapabilities = settingsProductCapabilities.Where(x => x.product_type == product.product_type).OrderBy(x => x.sort_order).ToList();
            var existingProductCapabilities = await _productCapabilitiesRepository.GetProductCapabilitiesFilters(request.ProductId);

            var productCapabilitiesViewModel = new ProductCapabilitiesViewModel
            {
                ProductId = product.product_id,
                ProductName = product.product_name,
                DraftAdditionalCapabilities = product.draft_other_capabilities,
                Status = (ProductStatus)product.status,
                Adb2CId = request.Adb2CId,
                ProductTypeId = product.product_type
            };

            var items = settingsProductCapabilities.Select(x => new SelectListItem
            {
                Text = x.capability_name,
                Value = x.capability_id.ToString()
            });

            //Assign existing filters
            var lstItems = items.ToList();
            foreach (var existingProductCapability in existingProductCapabilities)
            {
                for (var j = 0; j < lstItems.Count; j++)
                {
                    if (lstItems[j].Value == existingProductCapability.capability_id.ToString())
                    {
                        lstItems[j].Selected = true;
                    }
                }
            }

            productCapabilitiesViewModel.SettingsProductCapabilitiesList = lstItems;
            return productCapabilitiesViewModel;
        }

        public struct Context : IRequest<Optional<ProductCapabilitiesViewModel>>
        {
            public long ProductId { get; set; }

            public string Adb2CId { get; set; }
        }
    }
}