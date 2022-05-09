using AutoMapper;
using Beis.VendorManagement.Repositories.Interface;
using Beis.VendorManagement.Web.Models;
using Beis.VendorManagement.Web.Services.Interface;
using System.Threading.Tasks;

namespace Beis.VendorManagement.Web.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
    
        public ProductService(IMapper mapper, IProductRepository productRepository)
        {
            _mapper = mapper;
            _productRepository = productRepository;
        }

        public async Task<T> GetProduct<T>(long productId, string adb2CId)
        {
            var product = await _productRepository.GetProductSingle(productId, adb2CId);

            return _mapper.Map<T>(product);
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