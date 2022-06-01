using Notify.Client;

namespace Beis.VendorManagement.Web.Services
{
    public class NotifyService : INotifyService
    {
        private readonly IVendorCompanyService _vendorCompanyService;
        private readonly IAsyncNotificationClient _notificationClient;
        private readonly NotifyServiceOptions _options;

        public NotifyService(IVendorCompanyService vendorCompanyService, IOptions<NotifyServiceOptions> options)
            : this(vendorCompanyService, null, options)
        {
        }

        internal NotifyService(IVendorCompanyService vendorCompanyService, IAsyncNotificationClient notificationClient, IOptions<NotifyServiceOptions> options)
        {
            _vendorCompanyService = vendorCompanyService;
            _options = options.Value;

            if (string.IsNullOrWhiteSpace(_options.NotifyEmailKey))
                throw new ArgumentNullException(nameof(_options.NotifyEmailKey), "Cannot create notification client as api key is null");

            _notificationClient = notificationClient ?? new NotificationClient(_options.NotifyEmailKey);
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
                await _notificationClient.SendEmailAsync(user.Email, templateId, personalisation, default, default);
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
                await _notificationClient.SendEmailAsync(user.Email, templateId, personalisation, default, default);
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
                await _notificationClient.SendEmailAsync(emailAddress, templateId, personalisation, default, default);
            }
        }

        public class NotifyServiceOptions
        {
            public string NotifyEmailKey { get; set; }

            public string EmailVerificationLink { get; set; }
        }
    }
}