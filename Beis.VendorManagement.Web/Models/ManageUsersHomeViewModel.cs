namespace Beis.VendorManagement.Web.Models
{
    public class ManageUsersHomeViewModel : BaseViewModel
    {
        public IEnumerable<VendorCompanyUserViewModel> Users { get; set; }
    }
}