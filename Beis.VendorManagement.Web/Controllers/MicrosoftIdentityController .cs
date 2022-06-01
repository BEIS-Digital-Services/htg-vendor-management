using Microsoft.AspNetCore.Authentication;

namespace Beis.VendorManagement.Web.Controllers
{
    [AllowAnonymous]
    [Area("MicrosoftIdentity")]
    [Route("[area]/[controller]/[action]")]
    public class H2GAccountController : Controller
    {
        public IActionResult SignIn([FromRoute] string scheme)
        {
            return SetPolicyAndChallenge(scheme, "B2C_1_h2g_signin");
        }

        public IActionResult SignUp([FromRoute] string scheme)
        {
            return SetPolicyAndChallenge(scheme, "B2C_1_h2g_signup");
        }

        private IActionResult SetPolicyAndChallenge(string scheme, string policy)
        {
            scheme ??= OpenIdConnectDefaults.AuthenticationScheme;
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Content("~/"),
                Items =
                {
                    ["policy"] = policy
                }
            };

            return Challenge(properties, scheme);
        }
    }
}