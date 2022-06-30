namespace Beis.VendorManagement.Web.Services
{
    public class NotifyService : INotifyService
    {
        private readonly IVendorCompanyService _vendorCompanyService;
        private readonly IEmailClientService _emailClientService;
        private readonly NotifyServiceOptions _options;

        public NotifyService(IVendorCompanyService vendorCompanyService, IEmailClientService emailClientService, IOptions<NotifyServiceOptions> options)
        {
            _vendorCompanyService = vendorCompanyService;
            _emailClientService = emailClientService;
            _options = options.Value;
        }

        public async Task SendEmailNotification(long userId, IList<VendorCompanyUserViewModel> users, string templateId)
        {
            var company = await _vendorCompanyService.GetVendorCompany(users.First().CompanyId);
            var companyAddress = company.VendorCompanyAddress1;

            if (!string.IsNullOrWhiteSpace(company.VendorCompanyAddress2))
            {
                companyAddress = $"{companyAddress}, {company.VendorCompanyAddress2}";
            }

            companyAddress = $"{companyAddress}, {company.VendorCompanyCity}, {company.VendorCompanyPostcode}";

            var personalisation = new Dictionary<string, dynamic>
                                    {
                                        { "CompanyName", company.VendorCompanyName },
                                        { "CompanyAddress", companyAddress }
                                    };

            foreach (var user in users)
            {
                await _emailClientService.SendEmailAsync(user.Email, templateId, personalisation);
            }
        }

        public async Task SendEmailNotificationToAdditionalUsers(long userId, IEnumerable<VendorCompanyUserViewModel> users, string templateId)
        {
            var accessLink =  $"{_options.EmailVerificationLink}/ActivateAccount/CheckCompanyDetails/";

            foreach (var user in users)
            {
                var personalisation = new Dictionary<string, dynamic>
                                        {
                                            {"full name" , user.FullName},
                                            {"unique link" , $"{accessLink}{user.AccessLink}" }
                                        };
                await _emailClientService.SendEmailAsync(user.Email, templateId, personalisation);
            }
        }

        public async Task SendProductSubmittedConfirmationEmail(long userId, IEnumerable<string> emailAddresses, string templateId, string productName, string fullName, long companyId)
        {
            var company = await _vendorCompanyService.GetVendorCompany(companyId);
            var personalisation = new Dictionary<string, dynamic>
            {
                {"UserName" , fullName },
                {"VendorName" , company.VendorCompanyName },
                {"ProductName" , productName },
            };

            foreach (var emailAddress in emailAddresses)
            {
                await _emailClientService.SendEmailAsync(emailAddress, templateId, personalisation);
            }
        }

        public class NotifyServiceOptions
        {
            public string EmailVerificationLink { get; set; }
        }
    }
}