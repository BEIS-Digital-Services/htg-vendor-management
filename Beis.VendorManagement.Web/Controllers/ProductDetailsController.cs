using Beis.VendorManagement.Web.Constants;
using Beis.VendorManagement.Web.Handlers.ProductDetails;
using Beis.VendorManagement.Web.Models;
using Beis.VendorManagement.Web.Services.Interface;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Beis.VendorManagement.Web.Controllers
{
    [Authorize]
    public class ProductDetailsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IProductService _productService;

        public ProductDetailsController(IMediator mediator, IProductService productService)
        {
            _mediator = mediator;
            _productService = productService;
        }

        [HttpGet("Product/ProductSummary", Name = RouteNameConstants.ProductSummaryGet)]
        public async Task<IActionResult> ProductSummary(int id)
        {
            var summaryViewModel = await _productService.GetProduct<SummaryViewModel>(id, User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            summaryViewModel.ContentKey = $"Product-ProductSummary-{summaryViewModel.ProductName}";
            return View(summaryViewModel);
        }

        [HttpPost("Product/ProductSummary", Name = RouteNameConstants.ProductSummaryPost)]
        public async Task<IActionResult> ProductSummary(SummaryViewModel summaryViewModel)
        {
            if (!ModelState.IsValid)
            {
                summaryViewModel.ContentKey = $"Product-ProductSummary-error-{summaryViewModel.ProductName}";
                return View(summaryViewModel);
            }

            await _productService.UpdateSummary(summaryViewModel, User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return RedirectToRoute(RouteNameConstants.SoftwareHomeGet, new { id = summaryViewModel.ProductId });
        }

        [HttpGet("Product/ProductCapabilities", Name = RouteNameConstants.ProductCapabilitiesGet)]
        public async Task<IActionResult> ProductCapabilities(long id)
        {
            var result = await _mediator.Send(new ProductCapabilitiesGetHandler.Context { ProductId = id, Adb2CId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value });
            if (!result.HasValue)
            {
                return RedirectToRoute(RouteNameConstants.ProductErrorGet);
            }

            return View(result.Value);
        }

        [HttpPost("Product/ProductCapabilities", Name = RouteNameConstants.ProductCapabilitiesPost)]
        public async Task<IActionResult> ProductCapabilities(ProductCapabilitiesViewModel productCapabilitiesViewModel, long productId, long ProductTypeId)
        {
            await _mediator.Send(new ProductCapabilitiesPostHandler.Context
            {
                ProductId = productId, Adb2CId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value, ProductTypeId = ProductTypeId, ProductCapabilities = productCapabilitiesViewModel
            });

            return RedirectToRoute(RouteNameConstants.SoftwareHomeGet, new { id = productId });
        }

        [HttpGet("Product/ProductSupport", Name = RouteNameConstants.ProductSupportGet)]
        public async Task<IActionResult> ProductSupport(long id)
        {
            var result = await _mediator.Send(new ProductSupportGetHandler.Context { ProductId = id, Adb2CId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value });
            if (!result.HasValue)
            {
                return RedirectToRoute(RouteNameConstants.ProductErrorGet);
            }

            return View(result.Value);
        }

        [HttpPost("Product/ProductSupport", Name = RouteNameConstants.ProductSupportPost)]
        public async Task<IActionResult> ProductSupport(ProductSupportViewModel productSupportViewModel, int productId)
        {
            await _mediator.Send(new ProductSupportPostHandler.Context { ProductId = productId, ProductSupport = productSupportViewModel });
            return RedirectToRoute(RouteNameConstants.SoftwareHomeGet, new { id = productId });
        }

        [HttpGet("Product/PlatformDetails", Name = RouteNameConstants.PlatformDetailsGet)]
        public async Task<IActionResult> PlatformDetails(long id)
        {
            var result = await _mediator.Send(new PlatformDetailsGetHandler.Context { ProductId = id, Adb2CId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value });
            if (!result.HasValue)
            {
                return RedirectToRoute(RouteNameConstants.ProductErrorGet);
            }

            return View(result.Value);
        }

        [HttpPost("Product/PlatformDetails", Name = RouteNameConstants.PlatformDetailsPost)]
        public async Task<IActionResult> PlatformDetails(PlatformDetailsViewModel platformDetailsViewModel, int productId)
        {
            await _mediator.Send(new PlatformDetailsPostHandler.Context { ProductId = productId, Adb2CId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value, PlatformDetails = platformDetailsViewModel });
            return RedirectToRoute(RouteNameConstants.SoftwareHomeGet, new { id = productId });
        }
    }
}