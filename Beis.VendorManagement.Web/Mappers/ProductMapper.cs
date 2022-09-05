namespace Beis.VendorManagement.Web.Mappers
{
    internal static class ProductMapper
    {
        internal static RedemptionUrlViewModel MapRedemptionDetails(product product)
        {
            if (product == null)
            {
                return default;
            }

            return new RedemptionUrlViewModel
            {
                ProductId = product.product_id,
                ProductName = product.product_name,
                RedemptionUrl = product.redemption_url
            };
        }

        internal static SkuViewModel MapSkuDetails(product product)
        {
            if (product == null)
            {
                return default;
            }

            return new SkuViewModel
            {
                ProductId = product.product_id,
                ProductName = product.product_name,
                ProductSku = product.product_SKU
            };
        }

        internal static SummaryViewModel MapSummaryDetails(product product)
        {
            if (product == null)
            {
                return default;
            }

            return new SummaryViewModel
            {
                ProductId = product.product_id,
                ProductName = product.product_name,
                DraftProductDescription = product.draft_product_description,
                ProductStatus = (ProductStatus)product.status
            };
        }

        internal static ProductSubmitConfirmationViewModel MapSubmitConfirmationDetails(product product)
        {
            if (product == null)
            {
                return default;
            }

            return new ProductSubmitConfirmationViewModel
            {
                ProductId = product.product_id,
                ProductName = product.product_name
            };
        }

        internal static IEnumerable<product_filter> MapProductFilterDetails(IList<ProductFiltersModel> productFilters)
        {
            return productFilters.Select(r => new product_filter
            {
                product_id = r.ProductId,
                filter_id = r.FilterId,
                draft_filter = r.DraftFilter
            });
        }

        internal static IEnumerable<product_capability> MapProductCapabilityDetails(IList<ProductCapabilitiesModel> productCapabilities)
        {
            return productCapabilities.Select(r => new product_capability
            {
                product_id = r.ProductId,
                capability_id = r.CapabilityId,
                draft_capability = r.DraftCapability
            });
        }
    }
}