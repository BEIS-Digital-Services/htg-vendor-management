namespace Beis.HelpToGrow.Web.Services.Interface
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Beis.HelpToGrow.Web.Models;

    public interface INotifyService
    {
        Task SendEmailNotification(long userId, IList<VendorCompanyUserViewModel> emailPendingUsers, string templateId);
        
        Task SendEmailNotificationToAdditionalUsers(long userId, IEnumerable<VendorCompanyUserViewModel> emailPendingUsers, string templateId);

        Task SendProductSubmittedConfirmationEmail(long userId, IEnumerable<string> emailAddresses, string templateId, string productName, string fullName, long companyId);
    }
}