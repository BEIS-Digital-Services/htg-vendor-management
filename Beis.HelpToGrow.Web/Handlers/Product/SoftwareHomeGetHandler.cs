using Beis.HelpToGrow.Core.Repositories.Interface;
using Beis.HelpToGrow.Web.Models;
using Beis.Htg.VendorSme.Database.Models;
using MediatR;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Beis.HelpToGrow.Web.Handlers.Product
{
    public class SoftwareHomeGetHandler : IRequestHandler<SoftwareHomeGetHandler.Context, Optional<SoftwareHomeViewModel>>
    {
        private readonly IProductRepository _productRepository;
        private readonly ISettingsProductTypesRepository _settingsProductTypesRepository;
        private readonly IProductCapabilitiesRepository _productCapabilitiesRepository;
        private readonly ISettingsProductFiltersRepository _settingsProductFiltersRepository;
        private readonly IProductFiltersRepository _productFiltersRepository;

        public SoftwareHomeGetHandler(IProductRepository productRepository,
            ISettingsProductTypesRepository settingsProductTypesRepository,
            IProductCapabilitiesRepository productCapabilitiesRepository,
            ISettingsProductFiltersRepository settingsProductFiltersRepository,
            IProductFiltersRepository productFiltersRepository)
        {
            _productRepository = productRepository;
            _settingsProductTypesRepository = settingsProductTypesRepository;
            _productCapabilitiesRepository = productCapabilitiesRepository;
            _settingsProductFiltersRepository = settingsProductFiltersRepository;
            _productFiltersRepository = productFiltersRepository;
        }

        public async Task<Optional<SoftwareHomeViewModel>> Handle(Context request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Adb2CId))
            {
                return default;
            }

            var product = await _productRepository.GetProductSingle(request.ProductId, request.Adb2CId);
            if (product == null)
            {
                return default;
            }

            var softwareHome = new SoftwareHomeViewModel
            {
                Adb2CId = request.Adb2CId,
                ProductId = request.ProductId,
                ProductName = product.product_name,
                ProductStatus = (ProductStatus)product.status,
                DraftProductDescription = product.draft_product_description,
                ProductTypeName = (await _settingsProductTypesRepository.GetSettingsProductTypes())?.Where(x => x.id == product.product_type).First().item_name,
                HasCapabilities = await HasCapabilities(product),
                HasProductSupport = await CheckForProductSupport(product.product_id),
                HasPlatformDetails = await CheckForPlatformDetails(product.product_id)
            };

            softwareHome.CanSubmitForReview = softwareHome.HasCapabilities
                                              && !string.IsNullOrWhiteSpace(product.draft_product_description)
                                              && softwareHome.HasProductSupport && softwareHome.HasPlatformDetails;
            return softwareHome;
        }

        private async Task<bool> HasCapabilities(product product)
        {
            var existingProductCapabilities = await _productCapabilitiesRepository.GetProductCapabilitiesFilters(product.product_id);
            return existingProductCapabilities.Any() || product.other_capabilities != null;
        }

        private async Task<bool> CheckForPlatformDetails(long productId)
        {
            var filterTypes = new List<long> { (int)ProductFilterCategories.Platform };
            return await ValidateExistingFilters(productId, filterTypes);
        }

        private async Task<bool> CheckForProductSupport(long productId)
        {
            var filterTypes = new List<long> { (int)ProductFilterCategories.Support, (int)ProductFilterCategories.Training };
            return await ValidateExistingFilters(productId, filterTypes);
        }

        private async Task<bool> ValidateExistingFilters(long productId, ICollection<long> filterTypes)
        {
            var settingsProductFilters = await _settingsProductFiltersRepository.GetSettingsProductFilters(0);
            var defaultSettingsProductFilters = settingsProductFilters.Where(x => filterTypes.Contains(x.filter_type)).ToList();

            //Get existing filters
            var productFilters = await _productFiltersRepository.GetProductFilters(productId);

            foreach (var productFilter in productFilters)
            {
                foreach (var defaultSettingsProductFilter in defaultSettingsProductFilters)
                {
                    if (productFilter.filter_id == defaultSettingsProductFilter.filter_id)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public struct Context : IRequest<Optional<SoftwareHomeViewModel>>
        {
            public long ProductId { get; set; }

            public string Adb2CId { get; set; }
        }
    }
}