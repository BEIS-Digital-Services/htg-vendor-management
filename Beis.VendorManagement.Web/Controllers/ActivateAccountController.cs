namespace Beis.VendorManagement.Web.Controllers
{
    public class ActivateAccount : Controller
    {
        private readonly IActivateAccountService _service;
        private readonly ILogger<ActivateAccount> _logger;

        public ActivateAccount(IActivateAccountService service, ILogger<ActivateAccount> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("ActivateAccount/CheckCompanyDetails/{id}", Name = RouteNameConstants.CheckCompanyDetailsGet)]
        public async Task<ActionResult> CheckCompanyDetailsAsync(string id)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("SignOut", "Account", new { area = "MicrosoftIdentity" });
            }

            var user = await _service.GetUserByAccessLink(id);
            if (user == null)
            {
                _logger.LogError($"There is not an user with that guid: {id}");
                return RedirectToRoute(RouteNameConstants.ActivatedUserErrorGet);
            }


            var company = await _service.GetCompanyByUserIdAsync(user.UserId);
            company.PrimaryUserId = user.UserId;
            company.ContentKey = "ActivateAccount-CheckCompanyDetails";

            //Pass the activate account guid 
            TempData[ApplicationConstants.AccessLinkGuid] = id;

            return View(company);
        }

        [HttpPost("ActivateAccount/CheckCompanyDetails/{id}/{hasUserAuthorised}", Name = RouteNameConstants.CheckCompanyDetailsPost)]
        public async Task<ActionResult> CheckCompanyDetailsAsync(long id, bool hasUserAuthorised)
        {
            if(hasUserAuthorised)
                return RedirectToRoute(RouteNameConstants.TermsAndConditionsGet, new { id });

            var company = await _service.GetCompanyByUserIdAsync(id);
            company.PrimaryUserId = id;
            company.ShowErrorMessage = true;
            return View(company);
        }

        [HttpGet("ActivateAccount/TermsAndConditions", Name = RouteNameConstants.TermsAndConditionsGet)]
        public async Task<ActionResult> TermsAndConditionsAsync(long id)
        {
            var user = await _service.GetUserById(id);
            user.ContentKey = AnalyticConstants.ActivateAccountTermsAndConditions;
            return View(user);
        }

        [HttpPost("ActivateAccount/TermsAndConditions", Name = RouteNameConstants.TermsAndConditionsPost)]
        public async Task<ActionResult> TermsAndConditionsAsync(long id, bool hasTermsChecked, bool hasPrivacyPolicyChecked)
        {
            if(hasTermsChecked && hasPrivacyPolicyChecked)
                return RedirectToAction("SignUp", "H2GAccount", new { area = "MicrosoftIdentity" });

            var user = await _service.GetUserById(id);
            user.HasTermsChecked = hasTermsChecked;
            user.HasPrivacyPolicyChecked = hasPrivacyPolicyChecked;
            user.ShowErrorMessage = true;
            return View(user);
        }

        [HttpGet("ActivateAccount/TermsOfUse", Name = RouteNameConstants.TermsOfUseGet)]
        public ActionResult TermsOfUse()
        {
            return View();
        }

        [HttpGet("ActivateAccount/PrivacyPolicy", Name = RouteNameConstants.PrivacyPolicyGet)]
        public ActionResult PrivacyPolicy()
        {
            return View();
        }
    }
}