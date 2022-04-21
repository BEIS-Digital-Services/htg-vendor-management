using Beis.HelpToGrow.Web.Constants;
using Beis.HelpToGrow.Web.Models.Pricing;
using Beis.HelpToGrow.Web.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Beis.HelpToGrow.Web.Controllers
{
    [Authorize]
    public class PricingController : Controller
    {
        private readonly IProductService _productService;
        private readonly IPricingService _pricingService;

        public PricingController(IProductService productService, IPricingService pricingService)
        {
            _productService = productService;
            _pricingService = pricingService;
        }

        [HttpGet("Pricing/Home", Name = RouteNameConstants.PricingHomeGet)]
        public async Task<IActionResult> Home(int id, long productPriceId)
        {
            return View(new PricingHomeViewModel
            {
                ProductId = id,
                ProductPriceId = productPriceId,
                Adb2CId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                ProductName = await _productService.GetProductName(id, User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
            });
        }

        [HttpGet("Pricing/Metric", Name = RouteNameConstants.PricingMetricGet)]
        public async Task<IActionResult> Metric(int id, long productPriceId)
        {
            var metricDetails = await _pricingService.GetMetricDetails(id, User.FindFirst(ClaimTypes.NameIdentifier)?.Value, productPriceId);
            return View(metricDetails);
        }

        [HttpGet("Pricing/MinimumCommitment", Name = RouteNameConstants.PricingMinimumCommitmentGet)]
        public async Task<IActionResult> MinimumCommitment(int id, long productPriceId)
        {
            var minimumCommitment = await _pricingService.GetMinimumCommitment(id, User.FindFirst(ClaimTypes.NameIdentifier)?.Value, productPriceId);
            return View(minimumCommitment);
        }

        [HttpGet("Pricing/FreeTrial", Name = RouteNameConstants.PricingFreeTrialGet)]
        public async Task<IActionResult> FreeTrial(int id, long productPriceId)
        {
            var freeTrial = await _pricingService.GetFreeTrial(id, User.FindFirst(ClaimTypes.NameIdentifier)?.Value, productPriceId);
            return View(freeTrial);
        }

        [HttpGet("Pricing/DiscountPeriod", Name = RouteNameConstants.PricingDiscountPeriodGet)]
        public async Task<IActionResult> DiscountPeriod(int id, long productPriceId)
        {
            var discountPeriod = await _pricingService.GetDiscountPeriod(id, User.FindFirst(ClaimTypes.NameIdentifier)?.Value, productPriceId);
            return View(discountPeriod);
        }

        [HttpGet("Pricing/AdditionalDiscounts", Name = RouteNameConstants.PricingAdditionalDiscountsGet)]
        public async Task<IActionResult> AdditionalDiscounts(int id, long productPriceId)
        {
            var additionalDiscounts = await _pricingService.GetAdditionalDiscountsForPriceId(id, User.FindFirst(ClaimTypes.NameIdentifier)?.Value, productPriceId);
            return View(additionalDiscounts);
        }

        [HttpGet("Pricing/AdditionalCosts", Name = RouteNameConstants.PricingAdditionalCostsGet)]
        public async Task<IActionResult> AdditionalCosts(int id, long productPriceId)
        {
            var additionalCosts = await _pricingService.GetAdditionalCosts(id, User.FindFirst(ClaimTypes.NameIdentifier)?.Value, productPriceId);
            return View(additionalCosts);
        }

        [HttpGet("Pricing/ProductPricing", Name = RouteNameConstants.ProductPricingGet)]
        public async Task<IActionResult> ProductPricing(int id)
        {
            var productPriceDetails = await _pricingService.GetAllProductPrices(id, User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return View(productPriceDetails);
        }
    }
}