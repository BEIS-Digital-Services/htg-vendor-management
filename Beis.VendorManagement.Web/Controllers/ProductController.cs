﻿using Beis.VendorManagement.Web.Handlers.Product;

namespace Beis.VendorManagement.Web.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IProductService _productService;

        public ProductController(IMediator mediator, IProductService productService)
        {
            _mediator = mediator;
            _productService = productService;
        }

        [HttpGet("Product/SoftwareHome", Name = RouteNameConstants.SoftwareHomeGet)]
        public async Task<IActionResult> SoftwareHome(int id)
        {
            var result = await _mediator.Send(new SoftwareHomeGetHandler.Context { ProductId = id, Adb2CId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value });
            if (!result.HasValue)
            {
                return RedirectToRoute(RouteNameConstants.ProductErrorGet);
            }

            return View(result.Value);
        }

        [HttpGet("Product/RedemptionUrl", Name = RouteNameConstants.RedemptionUrlGet)]
        public async Task<IActionResult> RedemptionUrl(int id)
        {
            var redemptionUrlViewModel = await _productService.GetRedemptionUrlDetails(id, User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (redemptionUrlViewModel == null)
            {
                return RedirectToRoute(RouteNameConstants.ProductErrorGet);
            }

            redemptionUrlViewModel.ContentKey = $"{AnalyticConstants.ProductRedemptionUrl}{redemptionUrlViewModel.ProductName}";
            return View(redemptionUrlViewModel);
        }

        [HttpPost("Product/RedemptionUrl", Name = RouteNameConstants.RedemptionUrlPost)]
        public async Task<IActionResult> RedemptionUrl(RedemptionUrlViewModel redemptionUrlViewModel)
        {
            if (!ModelState.IsValid)
            {
                redemptionUrlViewModel = await _productService.GetRedemptionUrlDetails(redemptionUrlViewModel.ProductId, User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                redemptionUrlViewModel.ShowValidationError = true;
                redemptionUrlViewModel.ContentKey = $"{AnalyticConstants.ProductRedemptionUrlError}{redemptionUrlViewModel.ProductName}";
                return View(redemptionUrlViewModel);
            }

            await _productService.UpdateRedemptionUrl(redemptionUrlViewModel, User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return RedirectToRoute(RouteNameConstants.SoftwareHomeGet, new { id = redemptionUrlViewModel.ProductId });
        }

        [HttpGet("Product/TechnicalSolutions", Name = RouteNameConstants.ProductTechnicalSolutionsGet)]
        public IActionResult TechnicalSolutions()
        {
            return RedirectToRoute(RouteNameConstants.TechnicalSolutionsGet);
        }

        [HttpGet("Product/Sku", Name = RouteNameConstants.SkuGet)]
        public async Task<IActionResult> Sku(int id)
        {
            var skuViewModel = await _productService.GetSkuDetails(id, User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (skuViewModel == null)
            {
                return RedirectToRoute(RouteNameConstants.ProductErrorGet);
            }

            skuViewModel.ContentKey = $"{AnalyticConstants.ProductSku}{skuViewModel.ProductName}";
            return View(skuViewModel);
        }

        [HttpPost("Product/Sku", Name = RouteNameConstants.SkuPost)]
        public async Task<IActionResult> Sku(SkuViewModel skuViewModel)
        {
            await _productService.UpdateSku(skuViewModel, User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return RedirectToRoute(RouteNameConstants.SoftwareHomeGet, new { id = skuViewModel.ProductId });
        }

        [HttpGet("Product/ProductLogo", Name = RouteNameConstants.ProductLogoGet)]
        public async Task<IActionResult> ProductLogo(int id)
        {
            var result = await _mediator.Send(new ProductLogoGetHandler.Context { ProductId = id, Adb2CId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value, Host = Request.Host.Host, Scheme = Request.Scheme });
            if (!result.HasValue)
            {
                return RedirectToRoute(RouteNameConstants.ProductErrorGet);
            }

            return View(result.Value);
        }

        [HttpPost("Product/ProductLogo", Name = RouteNameConstants.ProductLogoPost)]
        public async Task<IActionResult> ProductLogo(ProductLogoViewModel productLogoViewModel)
        {
            if (!ModelState.IsValid)
            {
                productLogoViewModel.ContentKey = $"{AnalyticConstants.ProductLogoError}{productLogoViewModel.ProductName}";
                return View(productLogoViewModel);
            }

            await _mediator.Send(new ProductLogoPostHandler.Context { ProductId = productLogoViewModel.ProductId, Adb2CId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value, File = productLogoViewModel.File });
            return RedirectToRoute(RouteNameConstants.SoftwareHomeGet, new { id = productLogoViewModel.ProductId });
        }
    }
}