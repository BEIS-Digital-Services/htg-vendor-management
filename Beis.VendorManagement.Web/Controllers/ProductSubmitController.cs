using Beis.VendorManagement.Web.Constants;
using Beis.VendorManagement.Web.Handlers.ProductSubmit;
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
    public class ProductSubmitController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IProductService _productService;

        public ProductSubmitController(IMediator mediator, IProductService productService)
        {
            _mediator= mediator;
            _productService = productService;
        }

        [HttpGet("Product/ProductSubmitReviewDetails", Name = RouteNameConstants.ProductSubmitReviewDetailsGet)]
        public async Task<IActionResult> ProductSubmitReviewDetails(int id)
        {
            var result = await _mediator.Send(new ProductSubmitReviewDetailsGetHandler.Context { ProductId = id, Adb2CId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value, Host = Request.Host.Host, Scheme = Request.Scheme });
            if (!result.HasValue)
            {
                return RedirectToRoute(RouteNameConstants.ProductErrorGet);
            }

            return View(result.Value);
        }

        [HttpPost("Product/ProductSubmitReviewDetails", Name = RouteNameConstants.ProductSubmitReviewDetailsPost)]
        public async Task<IActionResult> ProductSubmitReviewDetails(ProductSubmitReviewDetailsViewModel productSubmitReviewDetailsViewModel)
        {
            var currentUserEmail = await _mediator.Send(new ProductSubmitReviewDetailsPostHandler.Context { Adb2CId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value, ProductSubmitReviewDetails = productSubmitReviewDetailsViewModel });
            return RedirectToRoute(RouteNameConstants.ProductSubmitConfirmationGet, new { id = productSubmitReviewDetailsViewModel.ProductId, email = currentUserEmail });
        }

        [HttpGet("Product/ProductSubmitConfirmation", Name = RouteNameConstants.ProductSubmitConfirmationGet)]
        public async Task<IActionResult> ProductSubmitConfirmation(int id, string email)
        {
            var productSubmitConfirmationViewModel = await _productService.GetProduct<ProductSubmitConfirmationViewModel>(id, User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (productSubmitConfirmationViewModel == null)
            {
                return RedirectToRoute(RouteNameConstants.ProductErrorGet);
            }

            productSubmitConfirmationViewModel.Email = email;
            productSubmitConfirmationViewModel.ContentKey = $"{AnalyticConstants.ProductSubmitConfirmation}{productSubmitConfirmationViewModel.ProductName}";
            return View(productSubmitConfirmationViewModel);
        }
    }
}