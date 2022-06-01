namespace Beis.VendorManagement.Web.Controllers
{
    [Authorize]
    public class HelpAndGuidanceController : Controller
    {
        [HttpGet("HelpAndGuidance/VendorPortalActivation", Name = RouteNameConstants.VendorPortalActivationGet)]
        public IActionResult VendorPortalActivation() => View();

        [HttpGet("HelpAndGuidance/ProductInformation", Name = RouteNameConstants.ProductInformationGet)]
        public IActionResult ProductInformation() => View();

        [HttpGet("HelpAndGuidance/Pricing", Name = RouteNameConstants.PricingGet)]
        public IActionResult Pricing() => View();
        
        [HttpGet("HelpAndGuidance/ProductComparisonTool", Name = RouteNameConstants.ProductComparisonToolGet)]
        public IActionResult ProductComparisonTool() => View();

        [HttpGet("HelpAndGuidance/TechnicalSolutions", Name = RouteNameConstants.TechnicalSolutionsGet)]
        public IActionResult TechnicalSolutions() => View();

        [HttpGet("HelpAndGuidance/ConfiguringYourApi", Name = RouteNameConstants.ConfiguringYourApiGet)]
        public IActionResult ConfiguringYourApi() => View();

        [HttpGet("HelpAndGuidance/ApiStandards", Name = RouteNameConstants.ApiStandardsGet)]
        public IActionResult ApiStandards() => View();

        [HttpGet("HelpAndGuidance/ContactUs", Name = RouteNameConstants.ContactUsGet)]
        public IActionResult ContactUs() => View();
    }
}