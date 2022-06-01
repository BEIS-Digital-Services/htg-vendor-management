using Microsoft.AspNetCore.Diagnostics;
using System.Diagnostics;

namespace Beis.VendorManagement.Web.Controllers
{
    public class ErrorController : Controller
    {
        [HttpGet("Error", Name = RouteNameConstants.ErrorGet)]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, ContentKey = AnalyticConstants.Error });

        [HttpGet("Error/LocalDevelopmentError", Name = RouteNameConstants.ErrorLocalDevelopmentGet)]
        public IActionResult ErrorLocalDevelopment([FromServices] IWebHostEnvironment webHostEnvironment)
        {
            if (webHostEnvironment.EnvironmentName != Environments.Development)
            {
                throw new InvalidOperationException(
                    "This shouldn't be invoked in non-development environments.");
            }

            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();

            return Problem(
                detail: context.Error.StackTrace,
                title: context.Error.Message);
        }

        [HttpGet("Error/LoggedUser", Name = RouteNameConstants.LoggedUserErrorGet)]
        public IActionResult LoggedUser() => View();

        [HttpGet("Error/InvalidCompany", Name = RouteNameConstants.InvalidCompanyErrorGet)]
        public IActionResult InvalidCompany() => View();

        [HttpGet("Error/ActivatedUser", Name = RouteNameConstants.ActivatedUserErrorGet)]
        public IActionResult ActivatedUser() => View();

        [HttpGet("Error/Product", Name = RouteNameConstants.ProductErrorGet)]
        public IActionResult Product() => View();
    }
}