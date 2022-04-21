using Beis.HelpToGrow.Web.Constants;
using Beis.HelpToGrow.Web.Handlers.Home;
using Beis.HelpToGrow.Web.Models;
using Beis.HelpToGrow.Web.Services.Interface;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Beis.HelpToGrow.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IAccountHomeService _accountHomeService;
        private readonly IMediator _mediator;

        public HomeController(IAccountHomeService accountHomeService, IMediator mediator)
        {
            _accountHomeService = accountHomeService;
            _mediator = mediator;
        }

        [HttpGet("", Name = RouteNameConstants.HomeIndexGet)]
        public async Task<ActionResult> IndexAsync()
        {
            if (!User.Identity.IsAuthenticated) return View();

            var result = await _mediator.Send(new IndexGetHandler.Context
            {
                AccessLinkId = TempData[ApplicationConstants.AccessLinkGuid]?.ToString(),
                UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                Email = User.FindFirst(ApplicationConstants.Emails)?.Value
            });

            if (!result.HasValue)
            {
                return RedirectToRoute(RouteNameConstants.ProductErrorGet);
            }

            TempData.Remove(ApplicationConstants.AccessLinkGuid);

            return View(result.Value.AccountHome);
        }

        [HttpGet("Home/Ranges", Name = RouteNameConstants.HomeRangesGet)]
        public async Task<ActionResult> Ranges()
        {
            var loggedInAdb2CId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            var company = await _accountHomeService.GetCompanyByUserIdAsync(loggedInAdb2CId);
            var rangesViewModel = new RangesViewModel
            {
                Adb2CId = loggedInAdb2CId,
                IpAddresses = company?.IpAddresses
            };

            return View(rangesViewModel);
        }

        [HttpPost("Home/Ranges", Name = RouteNameConstants.HomeRangesPost)]
        public async Task<ActionResult> Ranges(RangesViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return this.View(model);
            }

            var hasUpdated = await _accountHomeService.UpdateCompanyIpAddress(model.Adb2CId, model.IpAddresses.Trim());
            if (hasUpdated)
                return RedirectToRoute(RouteNameConstants.HomeIndexGet);

            this.ModelState.AddModelError(nameof(model.IpAddresses), "Unable to save ip addresses. Please try again later.");
            return this.View(model);

        }

        [HttpGet("Home/AccessibilityStatement", Name = RouteNameConstants.HomeAccessibilityStatementGet)]
        public IActionResult AccessibilityStatement()
        {
            return View(model: HttpContext.Request.Host.Value);
        }
    }
}