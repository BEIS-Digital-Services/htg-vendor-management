using Beis.VendorManagement.Web.Mappers;

namespace Beis.VendorManagement.Web.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
    
        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<RedemptionUrlViewModel> GetRedemptionUrlDetails(long productId, string adb2CId)
        {
            var product = await _productRepository.GetProductSingle(productId, adb2CId);
            return ProductMapper.MapRedemptionDetails(product);
        }

        public async Task<SkuViewModel> GetSkuDetails(long productId, string adb2CId)
        {
            var product = await _productRepository.GetProductSingle(productId, adb2CId);
            return ProductMapper.MapSkuDetails(product);
        }

        public async Task<SummaryViewModel> GetSummaryDetails(long productId, string adb2CId)
        {
            var product = await _productRepository.GetProductSingle(productId, adb2CId);
            return ProductMapper.MapSummaryDetails(product);
        }

        public async Task<ProductSubmitConfirmationViewModel> GetProductSubmitConfirmationDetails(long productId, string adb2CId)
        {
            var product = await _productRepository.GetProductSingle(productId, adb2CId);
            return ProductMapper.MapSubmitConfirmationDetails(product);
        }

        public async Task<string> GetProductName(long productId, string adb2CId)
        {
            var product = await _productRepository.GetProductSingle(productId, adb2CId);
            return product.product_name;
        }

        public async Task<bool> UpdateRedemptionUrl(RedemptionUrlViewModel redemptionUrlViewModel, string adb2CId)
        {
            var product = await _productRepository.GetProductSingle(redemptionUrlViewModel.ProductId, adb2CId);
            if (product == null) return false;

            product.redemption_url = redemptionUrlViewModel.RedemptionUrl;
            await _productRepository.UpdateProduct(product);
            return true;
        }

        public async Task<bool> UpdateSku(SkuViewModel skuViewModel, string adb2CId)
        {
            var product = await _productRepository.GetProductSingle(skuViewModel.ProductId, adb2CId);
            if (product == null) return false;

            product.product_SKU = skuViewModel.ProductSku;
            await _productRepository.UpdateProduct(product);
            return true;
        }

        public async Task<bool> UpdateSummary(SummaryViewModel summaryViewModel, string adb2CId)
        {
            var product = await _productRepository.GetProductSingle(summaryViewModel.ProductId, adb2CId);
            if (product == null) return false;

            product.draft_product_description = summaryViewModel.DraftProductDescription;
            await _productRepository.UpdateProduct(product);
            return true;
        }
    }
}