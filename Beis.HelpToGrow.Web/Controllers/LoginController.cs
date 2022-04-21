using Microsoft.AspNetCore.Mvc;

namespace Beis.HelpToGrow.Web.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult LandingPage()
        {
            return View();
        }
    }
}